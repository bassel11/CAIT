# Bin-Managed Warehouses Extension for WMS SRS

This document references `wms_srs.md` and details bin-level inventory management for warehouses. Bin management adds location granularity within warehouses (e.g., Aisle-Shelf-Bin).

## 1. Bin-Managed Warehouses Overview

**Definition**: Warehouses subdivided into hierarchical locations: Warehouse > Zone (e.g., picking/storage) > Aisle > Shelf > Bin. Bins are smallest unit for stock placement/movement.

**Key Principles**:
- Configurable hierarchy (ERP/WMS Manager sets up bins).
- Directed putaway/picking: System suggests optimal bins (e.g., FIFO, ABC).
- Capacity tracking per bin (volume/weight/item type).
- Multi-bin per item: Stock split across bins.
- License plate (bin label with barcode) for scanning.
- Cycle counts per bin.

**Supported Operations**:
- Inbound: Directed putaway (scan suggested bin or free-text).
- Outbound: Directed picking (pick list with bin locations).
- Transfers: Bin-to-bin moves.
- Counts: Bin-level blind/blindless counts.

**Configs** (per warehouse/item):
- Bin-enabled flag.
- Putaway strategy (nearest empty, fixed location).
- Picking strategy (zone-optimized, batch).
- Bin capacity limits/alerts.

## 2. Management Practices

**Live Setup** (Manager Web):
1. Live updates: Versioned hierarchy (v1→v2 soft rollout—new ops use new structure while legacy continues on old).
2. Define/update hierarchy: Warehouses > Zones > Aisles > Bins (barcodes).
3. Safety: Warn running business; optional ops pause/redirect.
2. Assign items to zones/bins (dynamic/fixed).
3. Set capacities/rules.

**Inbound (e.g., GRPO)**:
- Receive → System suggests bins (space/priority).
- Employee scans bin confirm or override.

**Outbound (e.g., Delivery)**:
- Pick list shows items with exact bins.
- Scan bin → item → qty.

**Inventory**:
- Transfers: Scan source/target bin.
- Counts: Generate bin sheets; variances auto-post.

**Best Practices**:
- ABC zoning: A-items near packing, C-items bulk.
- Dynamic allocation for variable demand.
- Barcode all bins/labels.
- Alerts for overcapacity.

**Data Model Add**:
- `wms_bin` (id, warehouse_id, zone, aisle, shelf, bin_code, capacity).
- `wms_stock_bin` (stock_id, bin_id, qty).

## 3. Modifications to Reference SRS (wms_srs.md)

**Section 1.3 Warehouse**: Add "Bins as sub-units within zones."

**Section 2.2 Key Features**: Add "- Bin-level location management (directed putaway/picking)."

**Section 3.1 Common Workflows**:
- Item Processing: Add "- Scan bin/location."
- Best Practices: Add "- Bin optimization (FIFO/ABC)."

**3.2 Inbound (all)**:
- Normal Flow: Add "Scan/suggest target bin" after item scan.
- Pre-conditions: "Bin-enabled warehouse."
- Alternative: "A2 Override bin suggestion."

**3.3 Outbound**:
- 3.3.1 Delivery: Pick list "with bin locations"; "Scan bin before item."
- Exceptions: "E1 Empty bin alert."

**3.3.4 Pick and Pack**: "Bin scan for pick verification."

**3.4.1 Inventory Transfer**: "Source/target bin scans."

**3.4.2 Counting**: "Bin-level sheets; location scan first."

**3.5 Dashboard**: Add "Bin occupancy KPIs; bin search."

**5. Data Model**: Add bin tables above.

**Terminology 8**: Add "- Bin: Smallest storage location (e.g., shelf slot) with barcode; supports directed ops."

