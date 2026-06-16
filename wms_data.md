# WMS data design for flexible bin-managed warehouses

## Goal
Allow warehouse managers to model **any bin hierarchy** they need while keeping:
1) **Fast bin lookup** (find bins by tree path / by barcode / by scanning).
2) **Correct attribute model** per storage type (e.g., **shelf attributes != aisle attributes != bin capacity attributes**).
3) Directed putaway/picking based on item attributes and bin constraints.

Examples:
- `warehouse → zones → aisles → shelves → bins`
- `warehouse → zones → aisles → bins`

---

## Core idea: “typed hierarchy” + “closure path”
Model the physical layout as a **typed tree** where every node is a *location element* (warehouse/zone/aisle/shelf/bin/… or custom).  
To support arbitrary user-defined depth (2nd example has no shelves), nodes are stored in a generic structure, but **each node has a type** that drives which attributes are valid.

To enable very fast “find bins in the tree” and assignment/suggestion, maintain:
- the **adjacency relationship** (parent_id)
- a **closure table** (ancestor/descendant + depth) so queries like “all bins under a zone/aisle” are fast.

---

## Entities (logical)

### 1) Warehouse & Location type
- `wms_warehouse` : the warehouse (physical or virtual)
- `wms_location_type` : allowed node types and their semantics
  - examples: `WAREHOUSE`, `ZONE`, `AISLE`, `SHELF`, `BIN`
  - can also include custom types (e.g., `LEVEL`, `ROW`, `PARKING_BAY`) depending on UI needs

### 2) Location nodes (tree)
Generic table for all hierarchy nodes:
- `wms_location_node`
  - `id`
  - `warehouse_id`
  - `type_id` (FK → `wms_location_type`)
  - `parent_id` (nullable for root; usually the warehouse node or a zone root)
  - `code` (human-readable like A01, L02, BIN-003)
  - `barcode` (optional; bin label often scanned, but you may allow scans at any level)
  - `is_active`, `sort_order`
  - `metadata` (optional JSON for small generic facts)

**Why generic?** Because user wants flexibility: some warehouses may omit shelf level.

### 3) Typed attributes per node
Different types need different columns. Two practical options:

**Option A (recommended): JSON attributes per node type**
- `wms_location_attr_def` defines attribute schema per type
  - (name, data_type, required, allowed_values, min/max, etc.)
- `wms_location_node_attr`
  - `node_id`
  - `attr_def_id`
  - `value_*` or `value_json`

**Option B: separate tables per type**
- `wms_shelf`, `wms_aisle`, `wms_bin` tables

Option A is more flexible and matches “user structures he needs”. Option B is simpler but less flexible.

### 4) Capacity and directed constraints
Bins have capacity (weight/volume), while aisles/shelves may have different constraints.
So define:
- `wms_bin_capacity` as an attribute set for nodes of type `BIN`

Implementation-wise, this can be either:
- stored as typed attributes in `wms_location_node_attr` (preferred for unified design), or
- kept in a dedicated `wms_bin_capacity` table (aligns with existing `wms_bin` idea from your docs).

### 5) Closure table for fast tree queries
To quickly answer:
- “list bins under this zone/aisle/shelf path”
- “given bin barcode, traverse to zone/aisle/shelf”

Use closure table:
- `wms_location_closure`
  - `ancestor_node_id`
  - `descendant_node_id`
  - `depth` (0 = same node)

Then a query becomes a single join: bins under node X are just all descendants of type BIN.

### 6) Stock in bins
Directed inventory assignment requires stock to live at **bin level**.
- `wms_stock_bin`
  - `stock_id` or `item_lot_serial_id` depending on your item tracking design
  - `bin_node_id` (FK → `wms_location_node` where type = BIN)
  - `qty_on_hand`
  - (optional) `reserved_qty`
  - (optional) `uom` conversion

This aligns with existing references (`wms_stock_bin` and `wms_bin`) in `bin_managed_warehouses.md`.

---

## Proposed relational database schema (SQL Server / Postgres friendly)
Below is a clean relational design that still allows custom hierarchies.

### Table: warehouses
```sql
CREATE TABLE wms_warehouse (
  id                BIGINT PRIMARY KEY,
  code              VARCHAR(50) NOT NULL UNIQUE,
  name              VARCHAR(200) NOT NULL,
  is_active         BIT NOT NULL DEFAULT 1
);
```

### Table: location types
```sql
CREATE TABLE wms_location_type (
  id                BIGINT PRIMARY KEY,
  code              VARCHAR(50) NOT NULL UNIQUE, -- e.g. 'WAREHOUSE','ZONE','AISLE','SHELF','BIN'
  name              VARCHAR(100) NOT NULL,
  is_terminal       BIT NOT NULL DEFAULT 0,       -- terminal for stock placement typically BIN
  default_sort_key VARCHAR(50) NULL
);
```

### Table: location tree nodes (generic)
```sql
CREATE TABLE wms_location_node (
  id                BIGINT PRIMARY KEY,
  warehouse_id      BIGINT NOT NULL REFERENCES wms_warehouse(id),
  type_id           BIGINT NOT NULL REFERENCES wms_location_type(id),
  parent_id        BIGINT NULL REFERENCES wms_location_node(id),

  code              VARCHAR(80) NOT NULL,         -- e.g., A01 / L02 / B003 (does not have to be unique globally)
  barcode           VARCHAR(120) NULL UNIQUE,    -- optional scan code (often only on BIN)

  sort_order        INT NOT NULL DEFAULT 0,
  is_active         BIT NOT NULL DEFAULT 1
);

-- Practical constraints:
-- - Usually BIN nodes are unique by (warehouse_id, code) (enforce if you want)
CREATE UNIQUE INDEX UX_wms_location_node_warehouse_type_code
ON wms_location_node(warehouse_id, type_id, code);
```

### Table: closure (ancestor/descendant)
```sql
CREATE TABLE wms_location_closure (
  ancestor_node_id    BIGINT NOT NULL REFERENCES wms_location_node(id),
  descendant_node_id  BIGINT NOT NULL REFERENCES wms_location_node(id),
  depth                INT NOT NULL,

  PRIMARY KEY (ancestor_node_id, descendant_node_id)
);

CREATE INDEX IX_wms_location_closure_descendant_depth
ON wms_location_closure(descendant_node_id, depth);
```

### Attribute definitions per type
```sql
CREATE TABLE wms_location_attr_def (
  id                BIGINT PRIMARY KEY,
  type_id           BIGINT NOT NULL REFERENCES wms_location_type(id),
  attr_key          VARCHAR(80) NOT NULL,    -- e.g., 'capacity_weight', 'capacity_vol', 'max_height'
  data_type         VARCHAR(30) NOT NULL,    -- 'number','string','bool','json','enum','date'
  is_required       BIT NOT NULL DEFAULT 0,
  min_value         DECIMAL(18,6) NULL,
  max_value         DECIMAL(18,6) NULL,
  enum_values_json  NVARCHAR(MAX) NULL       -- optional for enum validation
);

CREATE UNIQUE INDEX UX_wms_location_attr_def_type_key
ON wms_location_attr_def(type_id, attr_key);
```

### Attribute values for nodes
```sql
CREATE TABLE wms_location_node_attr (
  node_id           BIGINT NOT NULL REFERENCES wms_location_node(id),
  attr_def_id       BIGINT NOT NULL REFERENCES wms_location_attr_def(id),

  value_json        NVARCHAR(MAX) NULL,
  value_number      DECIMAL(18,6) NULL,
  value_string      NVARCHAR(4000) NULL,
  value_bool        BIT NULL,

  PRIMARY KEY(node_id, attr_def_id)
);
```

> This allows:
> - Aisle nodes to have aisle-level attributes (e.g., travel_profile, width_class)
> - Shelf nodes to have shelf-level attributes (e.g., max_weight_per_shelf, height)
> - Bin nodes to have capacity & putaway constraints

### Stock in bins
You already referenced `wms_stock_bin` in your docs.
Your exact `stock_id` depends on whether you store stock by item only or by item+batch+serial.
A common approach:
- keep an intermediate entity for tracked inventory.

Example minimal schema:
```sql
CREATE TABLE wms_stock_bin (
  stock_id     BIGINT NOT NULL,  -- points to your item/batch/serial stock ledger entity
  bin_node_id  BIGINT NOT NULL REFERENCES wms_location_node(id),

  qty_on_hand  DECIMAL(18,3) NOT NULL DEFAULT 0,
  reserved_qty DECIMAL(18,3) NOT NULL DEFAULT 0,

  PRIMARY KEY (stock_id, bin_node_id)
);

CREATE INDEX IX_wms_stock_bin_bin
ON wms_stock_bin(bin_node_id);
```

---

## How the system “finds bins in the tree easily”

### 1) Given a selected node (zone/aisle/shelf), get all bins
```sql
SELECT bn.id AS bin_node_id, bn.code, bn.barcode
FROM wms_location_closure c
JOIN wms_location_node bn
  ON bn.id = c.descendant_node_id
JOIN wms_location_type bt
  ON bt.id = bn.type_id
WHERE c.ancestor_node_id = :selected_node_id
  AND bt.is_terminal = 1; -- BIN type
```

Closure table makes this fast even for deep custom trees.

### 2) Given a scanned barcode, find bin and its ancestors
```sql
SELECT b.id AS bin_node_id
FROM wms_location_node b
WHERE b.barcode = :scanned_code
  AND b.type_id = :bin_type_id;

-- then to build path to zone/aisle/shelf for UI/validation:
SELECT anc.id, anc.type_id, anc.code
FROM wms_location_closure c
JOIN wms_location_node anc ON anc.id = c.ancestor_node_id
WHERE c.descendant_node_id = :bin_node_id
ORDER BY c.depth DESC; -- root to bin (or ASC depending on your convention)
```

### 3) Fast “bin suggestion” constrained by item
Directed putaway/picking needs:
- candidate bins under allowed zones
- bins that satisfy capacity constraints
- bins compatible with item constraints

You can implement a query pattern:
1) Filter candidate bins: bins under a configured zone for the warehouse.
2) Apply attribute constraints: `capacity_weight`, `item_allowed_categories`, etc.
3) Apply rotation rules: FIFO/FEFO/LIFO based on stock entries.

---

## Rotation / directed picking integration
Your docs mention FIFO/LIFO/FEFO directed logic.
To connect rotation with bins:
- Keep `wms_stock_bin` rows per bin per tracked-stock entity.
- Ensure your stock tracking entity includes `receipt_date` and/or `expiry_date`.
- Picking query:
  - Find bins that have available quantity for the item
  - Order candidates by rotation rule (FEFO expiry ASC, FIFO receipt ASC, LIFO receipt DESC)

This design stays valid regardless of whether shelves exist.

---

## Mapping to existing doc tables (`wms_bin`, `wms_stock_bin`)
Your current SRS references:
- `wms_bin (id, warehouse_id, zone, aisle, shelf, bin_code, capacity_..., barcode)`
- `wms_stock_bin (stock_id, bin_id, qty)`

With the “typed hierarchy” approach:
- `wms_bin` becomes a **view** over `wms_location_node` where `type_id = BIN`.
- `capacity_*` becomes typed attributes for BIN nodes.
- `zone/aisle/shelf` become ancestors retrieved via closure.

This gives you both:
- flexibility in hierarchy depth
- the ability to still build the UI and API contracts that expect bin_code + barcode.

---

## Example: two hierarchies coexisting
### Hierarchy 1: `Warehouse → Zones → Aisles → Shelves → Bins`
- Node types exist: ZONE, AISLE, SHELF, BIN

### Hierarchy 2: `Warehouse → Zones → Aisles → Bins` (no shelf)
- No SHELF nodes are created.
- Closure table still works: bins are descendants of the AISLE and ZONE.
- Attribute definitions for AISLE and BIN still apply.

No schema change is required; only data changes.

---

## What UI needs to configure (minimum)
1) Define `wms_location_type` and the attributes that belong to each type
2) Build the tree in the manager UI using drag/drop
3) Assign attributes (capacity/constraints) at the correct node types
4) Use barcode/label scanning to reference terminal bin nodes quickly

---

## Recommended conventions
- `barcode` unique across the warehouse (or globally)
- Enforce uniqueness: `(warehouse_id, type_id, code)`
- Treat BIN as terminal nodes (only those allow stock putaway)
- Maintain closure table during hierarchy updates (recompute for affected subtrees)

---

## Summary
Use:
- `wms_location_node` as a **typed, generic hierarchy tree**
- `wms_location_closure` for **fast ancestor/descendant bin lookup**
- `wms_location_attr_def` + `wms_location_node_attr` for **type-specific attributes**
- `wms_stock_bin` to store **inventory at terminal BIN nodes**

This satisfies both examples of custom hierarchy depth and enables fast bin search and directed assignment.

