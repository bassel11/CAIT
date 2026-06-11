# WMS (Bin Management & Warehouse Layout) — Implementation Proposal V3
### ITM.Core — SAP EWM–style Bin Management, aligned to the author's existing architecture
**Status:** Code-ready proposal — *nothing applied yet*. Every class below is final and complete; this document is the single source of truth before Phase 2 (migration generation).
**Author conventions verified against:** `Modules/Hr/*`, `OrganizationDbContext.cs`, `Abstracts/Entity.cs`, `Warehouse.cs`, `ProductAvailability.cs`, `InventoryTransactionHelper.cs`.

---

## 0. Executive Summary — What we kept, what we corrected from the Gemini draft

| # | Gemini's proposal | Verdict after reading the real code | Action in V3 |
|---|---|---|---|
| A | New namespace `ITM.Core.WMS.Database.Models` / `ITM.Core.WMS.Enums` | ❌ **Wrong.** No such project/namespace exists. It would break EF registration and the build. | ✅ Mirror the author's existing module pattern: `Modules/Wms/{Models,Enums,Configurations}`, namespace `ITM.Core.Organization.Database.Wms.*` |
| B | Distinguish bin-managed warehouses via `WarehouseType == BinLocation` (point 9) | ❌ **Architecturally unsafe.** `Warehouse.Type` is a single, mutually-exclusive enum. A `Rented` or `Consignment` warehouse could never *also* be bin-managed. | ✅ Add an **independent flag** `Warehouse.IsBinManaged` (SAP "WM-managed" flag). `Type` keeps its commercial meaning untouched. |
| C | "Hook already planted in `InventoryTransactionHelper`" | ❌ **Does not exist.** `SaveInventoryMovement` only updates the warehouse-level `ProductAvailability.OnHand`. | ✅ Documented as a **Phase-2 addition** (Appendix B), with the IM↔WM reconciliation invariant made explicit. |
| D | `ItemBinStock.ReservedQuantity` with no link to existing `ProductAvailability.Committed` | ⚠️ Potential double-source-of-truth. | ✅ Reconciliation invariant defined (§7) — bin-level `Reserved` rolls up to warehouse-level `Committed`. |
| E | Concurrency relies on `Entity.Version` (a one-time `string` GUID) | ⚠️ Not a real concurrency token; double-picking protection would be paper-thin. | ✅ Added a true `[Timestamp] RowVersion` on the hot `ItemBinStock` quant. |
| F | `ParentBinId` only (point 1) | ⚠️ Insufficient — risks Double-Counting in stock-take. | ✅ Kept Gemini's later fix: `BinSubdivisionState` enum + `MaxSubdivisions`, with the Putaway/Stock-take guard rules (§8). |
| G | `StorageType` / `StorageSection` naming | ✅ Safe. Note only: existing `StorageSystem`/`StorageSystemType` is **file storage** (S3/Azure/Local), unrelated — no clash, but flagged for the team. |

**Zero-Regression guarantee:** no existing class is deleted or re-typed. The *only* edit to an existing file is **one additive line** on `Warehouse.cs` (§9). Everything else is new files under `Modules/Wms/`.

---

## 1. Folder & namespace layout (mirrors `Modules/Hr` exactly)

```
ITM.Core.Organization.Database/
└── Modules/
    └── Wms/
        ├── Enums/
        │   ├── PutawayStrategy.cs
        │   ├── RemovalStrategy.cs
        │   ├── WmsTaskType.cs
        │   ├── WmsTaskStatus.cs
        │   ├── StorageTypeRole.cs
        │   ├── BinItemRestrictionType.cs
        │   ├── BinBatchRestrictionType.cs
        │   └── BinSubdivisionState.cs
        ├── Models/
        │   ├── StorageType.cs
        │   ├── StorageSection.cs
        │   ├── StorageBin.cs
        │   ├── ProductWarehouseData.cs
        │   ├── ItemBinStock.cs
        │   └── WmsWarehouseTask.cs
        └── Configurations/
            ├── StorageTypeConfiguration.cs
            ├── StorageSectionConfiguration.cs
            ├── StorageBinConfiguration.cs
            ├── ProductWarehouseDataConfiguration.cs
            ├── ItemBinStockConfiguration.cs
            ├── WmsWarehouseTaskConfiguration.cs
            └── WmsConfiguration.cs   // aggregator: ApplyWmsConfigurations()
```

> Convention confirmed in code: folder is `Modules/Hr` but the namespace **drops "Modules"** → `ITM.Core.Organization.Database.Hr.Models`. We follow the identical rule for `Wms`.

---

## 2. Enums  (`ITM.Core.Organization.Database.Wms.Enums`)

> Style matches the author: explicit integer values in steps of 10, no `[Flags]`, file-scoped behavior identical to `Modules/Hr/Enums/HrJobType.cs`.

### 2.1 `PutawayStrategy.cs`
```csharp
namespace ITM.Core.Organization.Database.Wms.Enums
{
    /// <summary>Inbound routing strategy: how the Putaway engine picks a destination bin.</summary>
    public enum PutawayStrategy
    {
        Manual = 0,
        NearestEmptyBin = 10,
        AdditionToExisting = 20,
        FixedBin = 30
    }
}
```

### 2.2 `RemovalStrategy.cs`
```csharp
namespace ITM.Core.Organization.Database.Wms.Enums
{
    /// <summary>Outbound picking strategy: which quant the engine proposes to the picker.</summary>
    public enum RemovalStrategy
    {
        FIFO = 10,
        LIFO = 20,
        FEFO = 30,
        StringentFIFO = 40
    }
}
```

### 2.3 `WmsTaskType.cs`
```csharp
namespace ITM.Core.Organization.Database.Wms.Enums
{
    /// <summary>Type of physical movement shown on the RF scanner / mobile app.</summary>
    public enum WmsTaskType
    {
        Putaway = 10,
        Picking = 20,
        BinTransfer = 30,
        PhysicalCount = 40,
        Replenishment = 50
    }
}
```

### 2.4 `WmsTaskStatus.cs`
```csharp
namespace ITM.Core.Organization.Database.Wms.Enums
{
    /// <summary>Lifecycle of a warehouse task.</summary>
    public enum WmsTaskStatus
    {
        Pending = 10,
        InProgress = 20,
        Completed = 30,
        Cancelled = 40
    }
}
```

### 2.5 `StorageTypeRole.cs`
```csharp
namespace ITM.Core.Organization.Database.Wms.Enums
{
    /// <summary>Functional/physical role of a storage zone and which operations it permits.</summary>
    public enum StorageTypeRole
    {
        Standard = 10,
        HighRack = 20,
        Bulk = 30,
        GoodsReceiptArea = 40,
        GoodsIssueArea = 50,
        StagingArea = 60,
        QualityInspection = 70
    }
}
```

### 2.6 `BinItemRestrictionType.cs`
```csharp
namespace ITM.Core.Organization.Database.Wms.Enums
{
    /// <summary>Prevents mixing incompatible products inside one bin (SAP B1 feature).</summary>
    public enum BinItemRestrictionType
    {
        None = 0,
        SpecificItem = 10,
        SpecificItemClass = 20
    }
}
```

### 2.7 `BinBatchRestrictionType.cs`
```csharp
namespace ITM.Core.Organization.Database.Wms.Enums
{
    /// <summary>Controls production-batch mixing within a single bin.</summary>
    public enum BinBatchRestrictionType
    {
        None = 0,
        SingleBatch = 10
    }
}
```

### 2.8 `BinSubdivisionState.cs`  *(the Double-Counting guard — §8)*
```csharp
namespace ITM.Core.Organization.Database.Wms.Enums
{
    /// <summary>
    /// Declares a bin's subdivision role so Putaway and Stock-Take queries stay correct.
    /// A ParentDivided bin must NEVER hold stock directly (would cause double counting).
    /// </summary>
    public enum BinSubdivisionState
    {
        None = 0,           // ordinary leaf bin: storable + counted
        ParentDivided = 10, // a divided parent: NOT storable, EXCLUDED from stock-take sums
        SubBin = 20         // a dynamically-created child section: storable + counted
    }
}
```

---

## 3. Entities  (`ITM.Core.Organization.Database.Wms.Models`)

> All inherit `Entity` (gives `Id, CreateDate, UpdateDate, CreatedBy, UpdatedBy, Version`), exactly like every `Hr*` model.
> Decimal columns use the author's `[Column(TypeName = ...)]` idiom. Cross-module references (`Warehouse`, `Product`, `ProductClass`, `UnitOfMeasurement`, `BatchNumber`, `SerialNumber`) point at the **existing** `ITM.Core.Organization.Database.Models` classes — verified to exist.

### 3.1 `StorageType.cs` — the Zone / Routing Brain
```csharp
using ITM.Core.Organization.Database.Abstracts;
using ITM.Core.Organization.Database.Models;
using ITM.Core.Organization.Database.Wms.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITM.Core.Organization.Database.Wms.Models
{
    /// <summary>
    /// A large zone/room inside a warehouse (e.g. Cold Room, Hazardous Zone, GR Area).
    /// Acts as the primary routing brain for inbound/outbound strategies.
    /// </summary>
    public class StorageType : Entity
    {
        public Guid WarehouseId { get; set; }
        public virtual Warehouse Warehouse { get; set; } = null!;

        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;

        public StorageTypeRole Role { get; set; } = StorageTypeRole.Standard;

        /// <summary>Lower number = searched first by the Putaway engine.</summary>
        public int SortOrder { get; set; }

        public PutawayStrategy PutawayStrategy { get; set; } = PutawayStrategy.NearestEmptyBin;
        public RemovalStrategy RemovalStrategy { get; set; } = RemovalStrategy.FIFO;

        /// <summary>If false, two different SKUs may never share a bin in this zone.</summary>
        public bool AllowMixedProducts { get; set; } = false;

        /// <summary>Enables tracking wrapped pallets (Handling Units / SSCC) as sealed entities.</summary>
        public bool IsHandlingUnitManaged { get; set; } = false;

        [Column(TypeName = "decimal(9,4)")]
        public decimal? MinTemperature { get; set; }

        [Column(TypeName = "decimal(9,4)")]
        public decimal? MaxTemperature { get; set; }

        public virtual ICollection<StorageSection> Sections { get; set; } = [];
    }
}
```

### 3.2 `StorageSection.cs` — ABC / Velocity subdivision
```csharp
using ITM.Core.Organization.Database.Abstracts;
using ITM.Core.Organization.Database.Models;
using ITM.Core.Organization.Database.Wms.Enums;

namespace ITM.Core.Organization.Database.Wms.Models
{
    /// <summary>
    /// Subdivides a StorageType by velocity (fast/slow movers) to minimise picker travel.
    /// </summary>
    public class StorageSection : Entity
    {
        public Guid StorageTypeId { get; set; }
        public virtual StorageType StorageType { get; set; } = null!;

        // Denormalised for hyper-fast warehouse-scoped queries (same intent as ItemBinStock).
        public Guid WarehouseId { get; set; }
        public virtual Warehouse Warehouse { get; set; } = null!;

        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;

        /// <summary>Section A (1) is filled before Section B (2), etc.</summary>
        public int SortOrder { get; set; }

        /// <summary>Optional override of the parent zone's inbound strategy.</summary>
        public PutawayStrategy? PutawayStrategyOverride { get; set; }

        public virtual ICollection<StorageBin> Bins { get; set; } = [];
    }
}
```

### 3.3 `StorageBin.cs` — the smallest scannable unit (+ nested/dynamic subdivision)
```csharp
using ITM.Core.Organization.Database.Abstracts;
using ITM.Core.Organization.Database.Models;
using ITM.Core.Organization.Database.Wms.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITM.Core.Organization.Database.Wms.Models
{
    /// <summary>
    /// The physical, barcode-scannable location where stock actually resides.
    /// </summary>
    public class StorageBin : Entity
    {
        public Guid StorageSectionId { get; set; }
        public virtual StorageSection StorageSection { get; set; } = null!;

        // Denormalised for fast warehouse-scoped queries.
        public Guid WarehouseId { get; set; }
        public virtual Warehouse Warehouse { get; set; } = null!;

        public string BinCode { get; set; } = null!;     // e.g. A-01-02-03
        public string? Description { get; set; }
        public string Barcode { get; set; } = null!;     // physical label scanned to confirm a task
        public string? BinType { get; set; }             // PalletBin, SmallBoxBin, ...

        // ----- Nested / dynamic subdivision (SAP EWM style) -----
        public Guid? ParentBinId { get; set; }
        public virtual StorageBin? ParentBin { get; set; }
        public virtual ICollection<StorageBin> ChildBins { get; set; } = [];
        public BinSubdivisionState SubdivisionState { get; set; } = BinSubdivisionState.None;
        /// <summary>Max dynamic child sections a parent may be split into (e.g. 2 half-pallets).</summary>
        public int? MaxSubdivisions { get; set; }

        // ----- Logical coordinates (human readable) -----
        public string? Aisle { get; set; }
        public string? Stack { get; set; }
        public string? Level { get; set; }

        // ----- Geometric coordinates (3D twin / route optimisation) -----
        [Column(TypeName = "decimal(18,6)")] public decimal? CoordX { get; set; }
        [Column(TypeName = "decimal(18,6)")] public decimal? CoordY { get; set; }
        [Column(TypeName = "decimal(18,6)")] public decimal? CoordZ { get; set; }

        // ----- Capacity limits (dynamic effective capacity = whichever fills first) -----
        [Column(TypeName = "decimal(18,6)")] public decimal MaxWeight { get; set; }
        [Column(TypeName = "decimal(18,6)")] public decimal MaxVolume { get; set; }
        [Column(TypeName = "decimal(18,6)")] public decimal MaxQuantity { get; set; }

        // ----- Automated replenishment thresholds -----
        [Column(TypeName = "decimal(18,6)")] public decimal? MinReplenishmentQuantity { get; set; }
        [Column(TypeName = "decimal(18,6)")] public decimal? MaxReplenishmentQuantity { get; set; }

        // ----- Overrides -----
        public RemovalStrategy? RemovalStrategyOverride { get; set; }

        /// <summary>If true, stock here is "in-transit/unsorted" and NOT sellable by Commerce.</summary>
        public bool IsReceivingBin { get; set; }
        public string? AlternateSortCode { get; set; }

        // ----- Item / batch / UoM restrictions -----
        public BinItemRestrictionType ItemRestrictionType { get; set; } = BinItemRestrictionType.None;
        public Guid? RestrictedToProductId { get; set; }
        public virtual Product? RestrictedToProduct { get; set; }
        public Guid? RestrictedToProductClassId { get; set; }
        public virtual ProductClass? RestrictedToProductClass { get; set; }

        public BinBatchRestrictionType BatchRestrictionType { get; set; } = BinBatchRestrictionType.None;
        public Guid? RestrictedToUoMId { get; set; }
        public virtual UnitOfMeasurement? RestrictedToUoM { get; set; }

        // ----- Blocks & holds -----
        public bool IsBlockedForPutaway { get; set; }
        public bool IsBlockedForPicking { get; set; }
        public string? BlockReason { get; set; }

        public virtual ICollection<ItemBinStock> Stocks { get; set; } = [];
    }
}
```

### 3.4 `ProductWarehouseData.cs` — logistics product master (1:1 with Product)
```csharp
using ITM.Core.Organization.Database.Abstracts;
using ITM.Core.Organization.Database.Models;

namespace ITM.Core.Organization.Database.Wms.Models
{
    /// <summary>
    /// Physical/logistics extension of the financial Product. Lets an item route itself.
    /// 1:1 with Product (enforced by a unique index on ProductId). Product.cs is untouched.
    /// </summary>
    public class ProductWarehouseData : Entity
    {
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        public bool RequiresColdStorage { get; set; }
        public bool IsHazardousMaterial { get; set; }

        /// <summary>Hard constraint: always prefer this zone for the item.</summary>
        public Guid? PreferredStorageTypeId { get; set; }
        public virtual StorageType? PreferredStorageType { get; set; }
    }
}
```

### 3.5 `ItemBinStock.cs` — the Quant (physical ledger, with real concurrency token)
```csharp
using ITM.Core.Organization.Database.Abstracts;
using ITM.Core.Organization.Database.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITM.Core.Organization.Database.Wms.Models
{
    /// <summary>
    /// The physical inventory ledger at bin granularity (the "quant").
    /// Separates free stock from stock already promised to a picker.
    /// INVARIANT: SUM(AvailableQuantity + ReservedQuantity) per (Product, Warehouse)
    ///            MUST equal ProductAvailability.OnHand for that pair (see §7).
    /// </summary>
    public class ItemBinStock : Entity
    {
        public Guid WarehouseId { get; set; }
        public virtual Warehouse Warehouse { get; set; } = null!;

        public Guid StorageBinId { get; set; }
        public virtual StorageBin StorageBin { get; set; } = null!;

        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        public Guid? BatchNumberId { get; set; }
        public virtual BatchNumber? BatchNumber { get; set; }

        public Guid? SerialNumberId { get; set; }
        public virtual SerialNumber? SerialNumber { get; set; }

        [Column(TypeName = "decimal(18,6)")] public decimal AvailableQuantity { get; set; }
        [Column(TypeName = "decimal(18,6)")] public decimal ReservedQuantity { get; set; }

        /// <summary>Timestamp the goods entered this bin — drives FIFO/LIFO.</summary>
        public DateTime EntryDate { get; set; }

        /// <summary>Denormalised expiry — drives FEFO without joining batch tables.</summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>SSCC of the full pallet / Handling Unit, if HU-managed.</summary>
        public string? HandlingUnitSscc { get; set; }

        /// <summary>True SQL Server rowversion — prevents concurrent double-picking.</summary>
        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;
    }
}
```

### 3.6 `WmsWarehouseTask.cs` — the executable movement order
```csharp
using ITM.Core.Organization.Database.Abstracts;
using ITM.Core.Organization.Database.Enums;     // reuse existing DocumentType
using ITM.Core.Organization.Database.Models;
using ITM.Core.Organization.Database.Wms.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITM.Core.Organization.Database.Wms.Models
{
    /// <summary>
    /// The physical execution order surfaced on the worker's RF scanner / mobile app.
    /// </summary>
    public class WmsWarehouseTask : Entity
    {
        public Guid WarehouseId { get; set; }
        public virtual Warehouse Warehouse { get; set; } = null!;

        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;

        // Ties the physical move to its originating financial document for full auditability.
        // Reuses the author's existing DocumentType enum (consistent with CheckBaseDocument).
        public Guid? BaseDocumentId { get; set; }
        public DocumentType? BaseDocumentType { get; set; }

        public WmsTaskType TaskType { get; set; }
        public WmsTaskStatus Status { get; set; } = WmsTaskStatus.Pending;

        public Guid? SourceBinId { get; set; }
        public virtual StorageBin? SourceBin { get; set; }

        public Guid? DestinationBinId { get; set; }
        public virtual StorageBin? DestinationBin { get; set; }

        [Column(TypeName = "decimal(18,6)")] public decimal TargetQuantity { get; set; }
        [Column(TypeName = "decimal(18,6)")] public decimal ActualQuantity { get; set; }

        public Guid? AssignedUserId { get; set; }

        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
```

---

## 4. EF Configurations  (`ITM.Core.Organization.Database.Wms.Configurations`)

> Each class is an `IEntityTypeConfiguration<T>` — identical style to `HrJobRoleConfiguration`.
> **Every** relationship is `DeleteBehavior.Restrict` (or `NoAction` where a cycle would otherwise be created) to honour the author's "no catastrophic cascade" rule (`OrganizationDbContext.cs:1059-1075`).
> Cross-module FKs use `.WithMany()` **without** an inverse navigation, so existing classes (`Warehouse`, `Product`, `ProductClass`, `UnitOfMeasurement`, `BatchNumber`, `SerialNumber`) need **no edits**.

### 4.1 `StorageTypeConfiguration.cs`
```csharp
using ITM.Core.Organization.Database.Wms.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITM.Core.Organization.Database.Wms.Configurations
{
    public class StorageTypeConfiguration : IEntityTypeConfiguration<StorageType>
    {
        public void Configure(EntityTypeBuilder<StorageType> builder)
        {
            builder.HasIndex(x => new { x.WarehouseId, x.Code }).IsUnique();

            builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(150).IsRequired();

            builder.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Sections)
                .WithOne(x => x.StorageType)
                .HasForeignKey(x => x.StorageTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
```

### 4.2 `StorageSectionConfiguration.cs`
```csharp
using ITM.Core.Organization.Database.Wms.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITM.Core.Organization.Database.Wms.Configurations
{
    public class StorageSectionConfiguration : IEntityTypeConfiguration<StorageSection>
    {
        public void Configure(EntityTypeBuilder<StorageSection> builder)
        {
            builder.HasIndex(x => new { x.StorageTypeId, x.Code }).IsUnique();

            builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(150).IsRequired();

            // StorageType -> Section relationship is owned on the StorageType side (4.1).

            // Denormalised warehouse FK: NoAction to avoid multiple cascade paths
            // (Warehouse -> StorageType -> Section AND Warehouse -> Section).
            builder.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(x => x.Bins)
                .WithOne(x => x.StorageSection)
                .HasForeignKey(x => x.StorageSectionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
```

### 4.3 `StorageBinConfiguration.cs`
```csharp
using ITM.Core.Organization.Database.Wms.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITM.Core.Organization.Database.Wms.Configurations
{
    public class StorageBinConfiguration : IEntityTypeConfiguration<StorageBin>
    {
        public void Configure(EntityTypeBuilder<StorageBin> builder)
        {
            // Barcode is the scan key — globally unique.
            builder.HasIndex(x => x.Barcode).IsUnique();
            builder.HasIndex(x => new { x.WarehouseId, x.BinCode }).IsUnique();

            builder.Property(x => x.BinCode).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Barcode).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.BinType).HasMaxLength(50);
            builder.Property(x => x.AlternateSortCode).HasMaxLength(100);
            builder.Property(x => x.BlockReason).HasMaxLength(500);
            builder.Property(x => x.Aisle).HasMaxLength(50);
            builder.Property(x => x.Stack).HasMaxLength(50);
            builder.Property(x => x.Level).HasMaxLength(50);

            // Self-reference for nested/dynamic bins — MUST be NoAction (no cascade cycle).
            builder.HasOne(x => x.ParentBin)
                .WithMany(x => x.ChildBins)
                .HasForeignKey(x => x.ParentBinId)
                .OnDelete(DeleteBehavior.NoAction);

            // Denormalised warehouse FK.
            builder.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.NoAction);

            // Restriction FKs to existing master data — no inverse navigations added.
            builder.HasOne(x => x.RestrictedToProduct)
                .WithMany()
                .HasForeignKey(x => x.RestrictedToProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.RestrictedToProductClass)
                .WithMany()
                .HasForeignKey(x => x.RestrictedToProductClassId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.RestrictedToUoM)
                .WithMany()
                .HasForeignKey(x => x.RestrictedToUoMId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Stocks)
                .WithOne(x => x.StorageBin)
                .HasForeignKey(x => x.StorageBinId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
```

### 4.4 `ProductWarehouseDataConfiguration.cs`
```csharp
using ITM.Core.Organization.Database.Wms.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITM.Core.Organization.Database.Wms.Configurations
{
    public class ProductWarehouseDataConfiguration : IEntityTypeConfiguration<ProductWarehouseData>
    {
        public void Configure(EntityTypeBuilder<ProductWarehouseData> builder)
        {
            // 1:1 with Product enforced by a unique index (Product.cs stays untouched).
            builder.HasIndex(x => x.ProductId).IsUnique();

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PreferredStorageType)
                .WithMany()
                .HasForeignKey(x => x.PreferredStorageTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
```

### 4.5 `ItemBinStockConfiguration.cs`
```csharp
using ITM.Core.Organization.Database.Wms.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITM.Core.Organization.Database.Wms.Configurations
{
    public class ItemBinStockConfiguration : IEntityTypeConfiguration<ItemBinStock>
    {
        public void Configure(EntityTypeBuilder<ItemBinStock> builder)
        {
            // One quant per (Bin, Product, Batch, Serial) — the natural key of the ledger.
            builder.HasIndex(x => new { x.StorageBinId, x.ProductId, x.BatchNumberId, x.SerialNumberId })
                .IsUnique();

            // Hot read path for putaway/picking lookups.
            builder.HasIndex(x => new { x.WarehouseId, x.ProductId });

            builder.Property(x => x.RowVersion).IsRowVersion();
            builder.Property(x => x.HandlingUnitSscc).HasMaxLength(50);

            builder.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.NoAction);

            // StorageBin -> Stocks owned on the bin side (4.3).

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.BatchNumber)
                .WithMany()
                .HasForeignKey(x => x.BatchNumberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SerialNumber)
                .WithMany()
                .HasForeignKey(x => x.SerialNumberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
```

### 4.6 `WmsWarehouseTaskConfiguration.cs`
```csharp
using ITM.Core.Organization.Database.Wms.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITM.Core.Organization.Database.Wms.Configurations
{
    public class WmsWarehouseTaskConfiguration : IEntityTypeConfiguration<WmsWarehouseTask>
    {
        public void Configure(EntityTypeBuilder<WmsWarehouseTask> builder)
        {
            // Worker queue + auditing access patterns.
            builder.HasIndex(x => new { x.WarehouseId, x.Status, x.TaskType });
            builder.HasIndex(x => new { x.BaseDocumentType, x.BaseDocumentId });

            builder.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SourceBin)
                .WithMany()
                .HasForeignKey(x => x.SourceBinId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.DestinationBin)
                .WithMany()
                .HasForeignKey(x => x.DestinationBinId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
```

### 4.7 `WmsConfiguration.cs` — aggregator (mirrors `HrConfiguration.ApplyHrConfigurations`)
```csharp
using Microsoft.EntityFrameworkCore;

namespace ITM.Core.Organization.Database.Wms.Configurations
{
    public static class WmsConfiguration
    {
        public static void ApplyWmsConfigurations(this ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StorageTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StorageSectionConfiguration());
            modelBuilder.ApplyConfiguration(new StorageBinConfiguration());
            modelBuilder.ApplyConfiguration(new ProductWarehouseDataConfiguration());
            modelBuilder.ApplyConfiguration(new ItemBinStockConfiguration());
            modelBuilder.ApplyConfiguration(new WmsWarehouseTaskConfiguration());
        }
    }
}
```

---

## 5. `OrganizationDbContext.cs` — the only context edits (additive)

**5.1 Add usings (top of file, next to the existing Hr usings at lines 3-4):**
```csharp
using ITM.Core.Organization.Database.Wms.Configurations;
using ITM.Core.Organization.Database.Wms.Models;
```

**5.2 Add DbSets (alongside the other DbSets, e.g. after `ProductAvailabilities` at line 471):**
```csharp
public virtual DbSet<StorageType> StorageTypes { get; set; }
public virtual DbSet<StorageSection> StorageSections { get; set; }
public virtual DbSet<StorageBin> StorageBins { get; set; }
public virtual DbSet<ProductWarehouseData> ProductWarehouseData { get; set; }
public virtual DbSet<ItemBinStock> ItemBinStocks { get; set; }
public virtual DbSet<WmsWarehouseTask> WmsWarehouseTasks { get; set; }
```

**5.3 One line inside `OnModelCreating`, right after the existing Hr call (`OrganizationDbContext.cs:1746`):**
```csharp
modelBuilder.ApplyHrConfigurations();
modelBuilder.ApplyWmsConfigurations();   // 👈 add this line
```

That is the **entire** footprint inside the context. No existing mapping is touched.

---

## 6. `Warehouse.cs` — the single additive line on an existing entity (Correction B)

Add **one** property (do **not** remove or retype anything). Place it next to `IsVirtual`:

```csharp
// existing:
public bool IsVirtual { get; set; } = false;

// 👈 ADD — independent WM-managed flag (SAP "Warehouse-Managed").
//     Decoupled from WarehouseType so ANY warehouse (Rented, Consignment, ...)
//     can be bin-managed without consuming its single Type value.
public bool IsBinManaged { get; set; } = false;
```

**Why not `WarehouseType.BinLocation`?** Because `Type` is single-valued: a warehouse cannot be both `Rented` *and* `BinLocation`. `IsBinManaged` is the orthogonal switch the WMS engine reads to decide IM-only vs WM (bin) processing — directly answering meeting **point 9**.

*(No EF configuration needed; a plain `bool` maps by convention, exactly like `IsVirtual`.)*

---

## 7. The IM ↔ WM reconciliation invariant (Corrections C & D)

Two ledgers now coexist and **must** stay consistent:

| Level | Entity | Field | Meaning |
|---|---|---|---|
| IM (warehouse) | `ProductAvailability` | `OnHand` | total physical units in the warehouse |
| IM (warehouse) | `ProductAvailability` | `Committed` | units promised to outbound docs |
| WM (bin) | `ItemBinStock` | `AvailableQuantity` | free units in a specific bin |
| WM (bin) | `ItemBinStock` | `ReservedQuantity` | units locked to a pending picking task |

**Invariant (per Product × Warehouse, only when `Warehouse.IsBinManaged == true`):**
```
Σ (ItemBinStock.AvailableQuantity + ItemBinStock.ReservedQuantity)  ==  ProductAvailability.OnHand
Σ (ItemBinStock.ReservedQuantity)                                    ==  ProductAvailability.Committed
```
`ProductAvailability` stays the **single source of financial truth**; `ItemBinStock` is the physical breakdown beneath it. The Phase-2 hook (Appendix B) must update both atomically in the same `SaveChanges`.

---

## 8. Subdivision business rules (Correction F) — enforced in the Phase-2 engine

To prevent Double-Counting, the application layer (not the schema) must guarantee:

1. **Putaway guard:** candidate-bin search **excludes** `SubdivisionState == ParentDivided`. Stock only ever lands in `None` or `SubBin`.
2. **Stock-take guard:** all `SUM`/rollup queries filter to `SubdivisionState != ParentDivided`.
3. **Un-split guard:** a bin may return `ParentDivided → None` **only** when every child `SubBin` is empty and removed.
4. **Dynamic split (SAP EWM HU flow):** on receiving an oversized HU into a `None` bin, the engine creates child `SubBin` rows (`A-01/1`, `A-01/2`), sets `ParentBinId = A-01`, flips `A-01` to `ParentDivided`, and locks it. On clearance it deletes the children and restores `A-01` to `None`. `MaxSubdivisions` caps the number of children.

---

## 9. Coverage matrix — the 14 meeting points vs. this schema

| # | Meeting point | Covered by | Status |
|---|---|---|---|
| 1 | Bin made of several bins | `ParentBinId` + `BinSubdivisionState` + `MaxSubdivisions` | ✅ |
| 2-3 | Bin↔Item link, dynamic capacity, barcode, transfer-on-change | `ItemBinStock` + `MaxWeight/MaxVolume/MaxQuantity` + unique `Barcode` + `WmsTaskType.BinTransfer` | ✅ |
| 4 | Oversize/overweight item (rack-break protection) | capacity check in `WmsValidationEngine` (Appendix A), `StorageTypeRole.Bulk` fallback | ✅ |
| 5 | Standalone Validation Engine (rules) | `WmsValidationEngine` pipeline (Appendix A) | ✅ (Phase 2) |
| 6 | Returns → Damaged/Inspection; Renting special | existing `Warehouse.DamagedGoodsWarehouseId`/`InspectionWarehouseId` + `StorageTypeRole.QualityInspection` | ✅ |
| 7 | Truck = 3 logical warehouses | each a `Warehouse` row, `IsBinManaged=false` | ✅ |
| 8 | Virtual/drop-ship triangular | existing `Warehouse.IsVirtual` + `WarehouseType.DropShip`, no WMS task generated | ✅ |
| 9 | Normal vs Bin | **`Warehouse.IsBinManaged`** (not `Type`) | ✅ (corrected) |
| 10 | Serial / Batch / Barcode / SSCC | `ItemBinStock.SerialNumberId/BatchNumberId` + `StorageBin.Barcode` + `ItemBinStock.HandlingUnitSscc` | ✅ |
| 11 | Labeling management | data ready (`BinCode/Barcode`); `WmsLabelingService` later | ✅ (Phase 3) |
| 12 | GRPO ≠ PO, document-triggered putaway | hook on real received qty (Appendix B) → `WmsValidationEngine` | ✅ (Phase 2) |
| 13 | Receipts exceed bin capacity (overflow) | route to `StorageTypeRole.StagingArea` overflow bin; never block financial receipt | ✅ |
| 14 | Offloading / two-step putaway | `StorageBin.IsReceivingBin` + `GoodsReceiptArea` then `WmsTaskType.Putaway` | ✅ |

---

## Appendix A — `WmsValidationEngine` (Phase 2, application layer — NOT in DB project)

Pipeline of independent rules; each returns pass/fail with a reason. Lives in the **Products** backend service layer (same place as `InventoryTransactionHelper`), reusing the existing `GetUOMQuantity` for volume conversion.

```csharp
public interface IBinRule
{
    Task<BinRuleResult> EvaluateAsync(BinPutawayContext ctx); // ctx: bin, product, qty, batch, current quant
}

// Order matters — fail fast:
//  1) BlockStatusRule        (IsBlockedForPutaway / Picking)
//  2) SubdivisionRule        (reject ParentDivided)
//  3) TemperatureRule        (ProductWarehouseData.RequiresColdStorage vs StorageType.Min/MaxTemperature)
//  4) HazardRule             (IsHazardousMaterial adjacency)
//  5) ItemRestrictionRule    (BinItemRestrictionType + RestrictedToProduct/Class)
//  6) BatchRestrictionRule   (BinBatchRestrictionType / RestrictedToUoM)
//  7) VolumetricCapacityRule (current + incoming <= MaxWeight/MaxVolume/MaxQuantity)
public class WmsValidationEngine
{
    private readonly IReadOnlyList<IBinRule> _rules; // DI-ordered
    public async Task<BinRuleResult> ValidateAsync(BinPutawayContext ctx)
    {
        foreach (var rule in _rules)
        {
            var r = await rule.EvaluateAsync(ctx);
            if (!r.Passed) return r;   // first failure wins → engine tries next candidate bin
        }
        return BinRuleResult.Ok();
    }
}
```

## Appendix B — Phase-2 integration hook (do NOT implement now)

The WMS posting attaches to the existing ledger update in
`ITM.Core.Products/Helper/InventoryTransactionHelper.cs : SaveInventoryMovement` (line ~293),
**immediately after** `ProductAvailability.OnHand` is updated, and **only when** `warehouse.IsBinManaged == true`:

```
GoodsReceipt:  resolve receiving bin (IsReceivingBin) → upsert ItemBinStock (+qty)
               → emit WmsWarehouseTask{ TaskType=Putaway, Base=GRPO }
GoodsIssues:   pick per RemovalStrategy (FIFO/FEFO via EntryDate/ExpiryDate)
               → decrement ItemBinStock under RowVersion concurrency
Always:        keep the §7 invariant true inside the same SaveChanges transaction.
```
For non-bin warehouses (`IsBinManaged == false`: trucks, virtual, drop-ship) the helper behaves **exactly as today** — zero regression.

---

## Phasing
1. **Phase 1 (this doc):** enums + entities + configurations + the 3 context edits + 1 Warehouse line → generate one migration.
2. **Phase 2:** `WmsValidationEngine` + Putaway/Picking services + the Appendix-B hook.
3. **Phase 3:** `WmsLabelingService` (ZPL/PDF), RF/mobile endpoints.
