# WMS Functional Scope (Software Requirements Specification - SRS) with Bin Management

*Integrated modifications from bin_managed_warehouses.md*


## 1. Introduction

### 1.1 Purpose
This document defines the complete functional scope for the Warehouse Management System (WMS) as a standalone module within itm-core, with optional ERP integration.

Key principles:
- Real-time operations in standalone mode; optional synchronization with external ERP systems (e.g., orders, POs, inventory).
- Role-based access: WMS Managers (web), WMS Employees (mobile, multi-warehouse).
- Barcode/SSCC/Serial/Batch management.
- Configurable features (e.g., allow non-PO items).
- Audit trails, notifications, conflict resolution (e.g., locked POs).
- Scalability for virtual warehouses within physical ones.
- Bins as sub-units within zones for granular location management.
- Mobile-first for operations: offline support with sync, camera for barcode scanning.

### 1.2 Scope
- **In Scope**: All listed operations (inbound/outbound/other), integrations, multi-warehouse, mobile/web split.
- **Out of Scope**: Hardware integration (e.g., RFID printers beyond barcode), advanced AI forecasting.
**Standalone Mode**: Local CRUD for POs/orders/inventory in WMS DB; optional bi-directional ERP sync via configurable APIs.
**Assumptions**: Mobile uses Flutter PWA/native; geolocation optional.

### 1.3 Users and Roles
- **WMS Manager**: Web dashboard for oversight, reports, configs.
- **WMS Employee**: Mobile app for operations; assigned to 1+ warehouses (physical/virtual).
- **ERP Admin**: System-wide configs (e.g., enable non-PO items).
- **Warehouse**: Physical + virtual sub-divisions (e.g., zones: picking, storage).

## 2. System Overview

### 2.1 Architecture
- **Web App**: React dashboard (ERP-integrated).
- **Mobile App**: flutter for iOS/Android; offline-first with IndexedDB sync.
- **Backend**: ASP.NET Core REST API + WebSockets for real-time (itm-core module).
- **Database**: Dedicated WMS DB (SQL Server/PostgreSQL); optional ERP sync.
- **Integrations**: ERP (POs, orders, inventory); Notification service (email/SMS/Push).

### 2.2 Key Features
- Multi-warehouse assignment per employee/item.
- PO filtering: Employees see only assigned warehouses' open POs/items.
- Locking mechanism: Prevent concurrent GRPO on same PO.
- Attachments/Notes per document.
  - Configs: Allow non-PO items, serial/batch management per item type.
  - Bin-level location management (directed putaway/picking).
  - Bin capacity tracking/alerts, hierarchy setup.

## 3. Functional Requirements

### 3.1 Common Workflows (All Operations)
1. **Login/Auth**: Role-based, multi-warehouse view.
2. **Dashboard**: Open POs/operations list, filtered by warehouses.
3. **PO/Operation Selection**: View header (matches ERP PO), contents (items filtered by warehouse).
4. **Item Processing**:
   - Enter qty received/issued.
   - Scan barcode (camera/Bluetooth).
   - Serials (if serial-managed): List/multiple scans.
   - Batch (if batch-managed): Expiry/Lot input.
   - SSCC (pallet-level): Optional scan.
  - Scan bin/location.
  - Non-PO item: Configurable add (reason dropdown: damage/extra).
5. **Validation**: Qty <= PO qty; serial uniqueness (unique item code + serial combination).
6. **Save/Submit**: Auto-lock PO; notify manager; ERP sync; audit log.
7. **Conflicts**: Locked PO warning, force-take confirmation (transfer lock + notify).
8. **Offline**: Queue actions, sync on reconnect.
9. **Attachments**: Upload photos/docs per GRPO/item.
10. **Notifications**: Push/email on submit, lock, conflicts.

**Best Practices**:
- Use GS1 standards for barcodes/SSCC.
- Real-time inventory deduction/prevention of oversell.
- Audit: All scans/changes logged with timestamp/user/warehouse/bin.
- Bin optimization (FIFO/ABC).

### 3.2 Inbound Operations

#### 3.2.1 GRPO (Goods Receipt PO)

**Pre-conditions**:
- User is authenticated WMS Employee assigned to relevant warehouse(s).
- PO exists locally or synced from ERP as 'Open' status.
- Employee's mobile device online (or offline queued).
- Configs: Serial/batch/SSCC enabled per item; non-PO add allowed.

**Post-conditions**:
- GRPO record created in ERP/WMS DB.
- PO status updated (e.g., 'Partially Received' or 'Closed').
- Inventory levels increased for received items (with serials/batches tracked).
- Lock released (unless partial draft).
- Audit log entry created.
- Manager notified via push/email.

**System Effects**:
- Local inventory update; optional ERP sync (prevent stock discrepancies).
- PO lock updated/removed.
- Notifications sent.
- Analytics updated (receipt metrics).

**Normal Flow**:
1. Employee logs in, views dashboard open POs filtered by warehouses.
2. Selects PO: Displays header (supplier/date/notes/attachments), items list (qty pending, warehouse-specific).
3. For each item: Enter received qty (≤ PO qty), scan barcode (matches item), add serials (if managed, unique validation), batch# (expiry if req), optional SSCC.
4. Optional: Add non-PO item (scan/select, qty, reason: extra/damage).
5. Add GRPO-level notes/attachments (photos).
6. Validate → Submit: Creates GRPO, updates ERP.

**Alternative Flows**:
- **A1 Partial Receipt**: Save draft (status 'Draft') → Reopen later from dashboard.
- **A2 Non-PO Add**: Enabled config → Add item row, process as normal.
- **A3 Locked PO**: System detects lock → Show warning ("Locked by [User] since [time]") → Confirm force → Transfer lock, notify previous user.
- **A4 Over-Receipt**: Qty > PO + tolerance → Warning → Manager approval req.
- **A5 Offline**: Queue locally → Auto-sync on reconnect.

**Exceptions**:
- E1 Barcode mismatch: Error message, retry.
- E2 Serial duplicate (item code + serial): Reject, list conflicts.

- E3 Sync fail: Retry/queue, alert user.

**Reports**: Integrated dashboard (receipt accuracy by supplier/item, variances).

#### 3.2.2 Sales Return (Sales Return Inbound)

**Pre-conditions**:
- User is authenticated WMS Employee assigned to relevant warehouse(s).
- Sales Return Order exists in ERP as 'Open' status, linked to warehouse.
- Device online (or offline queued).
- Configs: Quarantine zone defined; reason codes enabled.

**Post-conditions**:
- Return Receipt record created in ERP/WMS DB.
- Sales Return Order status updated (e.g., 'Partially Returned' or 'Closed').
- Inventory adjusted: Items to quarantine/inspection warehouse or restock.
- Serials/batches updated or invalidated.
- Audit log created.
- Credit note auto-generated if full return (configurable).

**System Effects**:
- ERP inventory sync (negative adjustment or quarantine move).
- Return Order lock managed.
- Notifications to customer service/manager.
- Analytics: Return rates by reason/product.

**Normal Flow**:
1. Employee views dashboard open Sales Returns filtered by warehouses.
2. Selects Return Order: View header (customer/date/reason/notes), items list.
3. Per item: Scan barcode, enter qty returned, reason code (mandatory: damage/defect/wrong-item), serials/batch if applicable, optional SSCC.
4. Add notes/attachments (photos of damage).
5. Validate → Submit: Processes return, updates ERP.

**Alternative Flows**:
- **A1 Partial Return**: Save draft → Reopen.
- **A2 Quarantine**: Auto-move to quarantine zone based on reason.
- **A3 Locked**: Warning + force-take.
- **A4 Credit Note**: Full qty → Auto-trigger credit process.
- **A5 Restock**: Non-defective → Back to available inventory.

**Exceptions**:
- E1 Invalid reason: Dropdown validation.

#### 3.2.3 Goods Receipt (General)

**Pre-conditions**:
- User is authenticated WMS Employee assigned to source/target warehouse(s).
- Device online (or offline queued).
- Configs: Serial/batch/SSCC enabled per item; non-PO allowance (always on for GR).

**Post-conditions**:
- GR record created in ERP/WMS DB.
- Inventory adjusted: Increase in target warehouse (serials/batches tracked).
- Audit log created.
- Optional: Manager notification if value > threshold (configurable).

**System Effects**:
- ERP inventory sync (add to target, deduct from source if transfer).
- Analytics updated (ad-hoc receipt metrics).

**Normal Flow**:
1. Employee selects "New Goods Receipt" from dashboard (or search).
2. Enter GR header: Title/date/target warehouse/source (optional: internal/other), notes.
3. Add items: Manual select/scan barcode, qty, serials/batch/SSCC if applicable.
4. Optional: Reason (donation/internal transfer/correction), attachments (photos).
5. Validate → Submit: Creates GR, updates inventory.

**Alternative Flows**:
- **A1 Partial GR**: Save draft → Reopen from dashboard.
- **A2 Transfer Mode**: Source warehouse specified → Auto-deduct inventory.
- **A3 Quarantine**: Configurable for certain reasons → Move to quarantine zone.
- **A4 Attachments**: Upload per GR/item.

**Exceptions**:
- E1 Serial duplicate: Reject.
- E2 Inventory insufficient (transfers): Warning/approval.

**Reports**: Ad-hoc receipts summary (by reason/warehouse).

#### 3.2.4 Receipt from Production

**Pre-conditions**:
- User is authenticated WMS Employee assigned to production/target warehouse(s).
- Production Order exists in ERP as 'Completed' status, linked to warehouse.
- Device online (or offline queued).
- Configs: Batch/lot mandatory for produced items.

**Post-conditions**:
- Production Receipt record created in ERP/WMS DB.
- Production Order status updated to 'Received'.
- Inventory increased in target warehouse (raw/finished goods, batches tracked).
- Audit log created.
- Production manager notified.

**System Effects**:
- ERP inventory sync (add produced qty/batches).
- Analytics: Production yield rates.

**Normal Flow**:
1. Employee views dashboard open Production Orders filtered by warehouses.
2. Selects Production Order: View header (product/date/qty expected), produced items/batches.
3. Per item: Scan barcode, confirm produced qty/batch/lot (auto-populate from prod order), optional SSCC.
4. Add notes/attachments (quality photos).
5. Validate → Submit: Processes receipt, updates inventory.

**Alternative Flows**:
- **A1 Partial Receipt**: Save draft for partial yield.
- **A2 Quality Hold**: Flag batches for inspection/quarantine.
- **A3 Over-Yield**: Tolerance check, manager approval.
- **A4 Offline**: Queue sync.

**Exceptions**:
- E1 Batch mismatch: Validation against prod order.
- E2 Sync fail: Retry.

**Reports**: Production receipts vs. planned, yield variances.

### 3.3 Outbound Operations

#### 3.3.1 Delivery

**Pre-conditions**:
- User is authenticated WMS Employee assigned to relevant warehouse(s).
- Sales Delivery Order exists in ERP as 'To Pick' status.
- Device online (or offline queued).
- Configs: Location/bin enabled; wave picking optional.

**Post-conditions**:
- Delivery record updated in ERP/WMS DB (picked/packed/issued).
- Delivery Order status to 'Ready to Ship'.
- Inventory deducted (with serials/batches/SSCC tracked).
- Audit log created.
- Shipping notified.

**System Effects**:
- ERP inventory real-time deduction.
- Pick accuracy analytics.

**Normal Flow**:
1. Employee views dashboard open Deliveries filtered by warehouses.
2. Selects Delivery: View pick list (items/qty by bin/location).
3. Pick phase: Scan location/bin, scan items (qty <= allocated).
4. Pack phase: Assign to SSCC/pallet, scan confirm.
5. Issue → Submit: Deducts inventory, prints labels.

**Alternative Flows**:
- **A1 Wave Picking**: Batch multiple deliveries.
- **A2 Short Pick**: Variance reason/approval.
- **A3 Locked**: Force-take.
- **A4 Partial Pack**: Save partial.

**Exceptions**:
- E1 Item not in bin: Relocate prompt.
- E2 Stock short: Partial pick alert.

**Reports**: Pick accuracy, on-time delivery.

#### 3.3.2 Goods Return PO

**Pre-conditions**:
- User is authenticated WMS Employee assigned to relevant warehouse(s).
- PO Return exists in ERP (linked to original PO).
- Device online (or offline queued).

**Post-conditions**:
- Return Issue record created.
- Inventory deducted from warehouse, prepared for supplier.
- PO status updated.
- Audit log created.

**System Effects**:
- ERP inventory adjustment (outbound).
- Supplier notified.

**Normal Flow**:
1. View open PO Returns dashboard.
2. Select PO Return: View items to return.
3. Pick/scan items/serials from warehouse.
4. Pack to SSCC.
5. Submit: Updates ERP.

**Alternative Flows**:
- **A1 Partial Return**: Draft save.
- **A2 Inspection**: Quality check hold.

**Exceptions**:
- E1 Item missing: Reason log.

#### 3.3.3 Goods Issue

**Pre-conditions**:
- User is authenticated WMS Employee assigned to warehouse.
- No specific order; ad-hoc (e.g., service, internal).

**Post-conditions**:
- Issue record created.
- Inventory deducted.
- Recipient noted.

**System Effects**:
- Inventory sync.
- Costing tracked.

**Normal Flow**:
1. Select "New Goods Issue" dashboard.
2. Enter header: Recipient/reason.
3. Select/scan items/qty from locations.
4. Submit.

**Alternative Flows**:
- **A1 Approval**: High-value requires manager ok.
- **A2 Serial Track**: Mandatory for certain items.

**Exceptions**:
- E1 Insufficient stock.

**Reports**: Issue history by reason.

#### 3.3.4 Pick and Pack

**Pre-conditions**:
- Part of delivery process; pick list generated.

**Post-conditions**:
- Items picked/packed, SSCC assigned.

**System Effects**:
- Staging inventory update.

**Normal Flow**:
1. Generate pick list (web/manager).
2. Employee scans pick: Location → Item confirm.
3. Pack: Group to box/pallet, generate SSCC/label.

**Alternative Flows**:
- **A1 Voice Picking**: Audio guidance (future).
- **A2 Bulk Pack**: Multiple orders.

**Exceptions**:
- E1 Pick error: Override with reason.

**Reports**: Pack efficiency.

### 3.4 Other Operations

#### 3.4.1 Inventory Transfer

**Pre-conditions**:
- User is authenticated WMS Employee assigned to source/target warehouses.
- Sufficient stock in source location/warehouse.
- Device online (or offline queued).

**Post-conditions**:
- Transfer record created.
- Inventory adjusted (deduct source, add target).
- Audit log created.

**System Effects**:
- Real-time inventory sync across warehouses.
- Location history updated.

**Normal Flow**:
1. Select "New Inventory Transfer" dashboard.
2. Enter header: Source/target warehouse/zone/location, reason.
3. Select/scan items/qty from source.
4. Scan target location confirm.
5. Submit: Updates inventory.

**Alternative Flows**:
- **A1 Partial Transfer**: Qty split.
- **A2 Approval**: Cross-physical warehouse requires manager.

**Exceptions**:
- E1 Insufficient source stock.

**Reports**: Transfer history, stock movement.

#### 3.4.2 Counting (Cycle Count/Full Inventory)

**Pre-conditions**:
- Manager generates count sheet (scheduled/random).
- Employee assigned to count.

**Post-conditions**:
- Count record updated; variances adjusted/approved.
- Inventory corrected.

**System Effects**:
- Accuracy analytics.
- Adjustment postings.

**Normal Flow**:
1. Employee opens count sheet (blind or with expected).
2. Scan location/items, enter counted qty.
3. Submit variances → Manager approval.
4. Adjust inventory.

**Alternative Flows**:
- **A1 Blind Count**: No expected qty shown.
- **A2 Recount**: Variance > threshold.

**Best Practices**:
- ABC: Frequent high-value.
**Exceptions**:
- E1 Access denied location.

**Reports**: Count accuracy trends.

#### 3.4.3 Pick List

**Pre-conditions**:
- Manager generates for delivery/outbound.

**Post-conditions**:
- Printable/digital list available.

**System Effects**:
- Picking guidance provided.

**Normal Flow**:
1. Web dashboard: Filter orders → Generate pick list (PDF/print).
2. Employee loads on mobile.

**Alternative Flows**:
- **A1 Optimized Route**: Zone-based sequencing.

**Exceptions**:
- None.

**Reports**: Usage stats.

#### 3.4.4 Package

**Pre-conditions**:
- Items picked, ready for packing.

**Post-conditions**:
- Packages labeled/SSCC assigned.

**System Effects**:
- Manifest updated.

**Normal Flow**:
1. Scan picked items into staging.
2. Group to box/pallet, generate SSCC/label.
3. Seal/confirm.

**Alternative Flows**:
- **A1 Fragile Handling**: Special labels.

**Exceptions**:
- E1 Weight/volume exceed.

**Reports**: Packaging efficiency.

### 3.5 Manager Web Dashboard
- **Views**:
  - Real-time KPIs: Inbound/outbound volume, inventory levels, pick accuracy.
  - Reports: By warehouse/operation/date; export CSV/PDF.
  - Operations Queue: Pending GRPOs/drafts/locks.
  - Configs: Employee-warehouse assignments, tolerances, non-PO allowance.
- **Notifications**: Alerts (e.g., lock disputes, variances > threshold).

## 4. Non-Functional Requirements
- **Performance**: <2s scan response; handle 100 concurrent users.
- **Security**: ERP auth (JWT); data encryption; audit immutable.
- **Usability**: Intuitive mobile UI; multilingual.
- **Reliability**: 99.9% uptime; auto-backup syncs.
- **Scalability**: Cloud-ready (e.g., AWS/Azure).

## 5. Data Model Extensions (ERP)
- Tables: wms_grpo, wms_lock (po_id, user, timestamp), wms_audit.
- Fields: sscc, serials (JSON array), batch_details.

## 6. Testing Scenarios
- Unit: Scan validation.
- Integration: ERP sync.
- E2E: Full GRPO → Inventory update.
- Edge: Offline, locks, non-PO.

## 7. Deployment
- Mobile: App stores/PWA.
- Web: ERP-integrated.

This SRS serves as the definitive reference; iterate based on mockup reviews.

## 8. Terminology

- **Serial Number**: Unique identifier for individual trackable items (e.g., electronics). Enables precise inventory tracking, one-by-one scanning/validation.
- **Batch Number**: Group identifier for items produced/manufactured together (e.g., pharmaceuticals). Tracks production run, shared expiry dates, recalls.
- **SSCC (Serial Shipping Container Code)**: GS1 standard 18-digit barcode for logistics units (pallets/boxes). Enables full supply chain traceability from supplier to customer.
- **Barcode**: Scannable code (1D/2D like Code128, QR) on items/containers for quick identification/input.
- **GRPO**: Goods Receipt Purchase Order - Inbound receipt against a PO.
- **PO**: Purchase Order - ERP document for supplier inbound.
- **SSCC**: See above.
- **ABC Analysis**: Inventory categorization (A: high value/frequent, B: medium, C: low) for optimized counting/risk management.
- **Wave Picking**: Batch processing multiple orders simultaneously for efficiency.
- **Blind Count**: Physical inventory count without prior expected quantities shown, reduces bias.

