# تدقيق SRS 6 (WMS with Bin Management) — الحالة الفعلية في الكود ومعالجتها وفق SAP B1 10.0

> **مرجع المصدر:** `D:\ITM\srs\_srs 6.md`
> **نطاق التدقيق الفعلي:** `ITM.Core.Products.Backend` + `ITM.Core.Commerce.Backend` + `ITM.Core.Organization.Database`
> **تاريخ التدقيق:** 2026-09-09
> **منهجية:** كل بند أدناه مبني على **قراءة كود فعلية** (مع مرجع ملف/سطر)، وليس افتراضًا. البنود المتعلقة بالصلاحيات/الأدوار استُبعدت بناءً على طلبك.

---

## 0. الرموز المستخدمة

| الرمز | المعنى |
|---|---|
| ✅ | موجود ومكتمل منطقيًا |
| ⚠️ | موجود جزئيًا (نموذج بيانات موجود / منطق ناقص، أو منطق موجود بثغرة) |
| ❌ | غير موجود إطلاقًا (صفر كود) |
| 🚫 | خارج النطاق بطلبك (صلاحيات/أدوار) أو خارج نطاق SAP B1 الأساسي |
| 🔍 | يحتاج تأكيدًا منك من حساب SAP B1 10.0 |

---

## 1. الخلاصة التنفيذية (اقرأ هذا أولًا)

**البنية التحتية أقوى بكثير مما يوحي به الـSRS، لكن "محرّك القرار" (Decision Engine) غائب.**

الموجود فعليًا وبجودة جيدة:
- شجرة مواقع كاملة (Warehouse → Zone → Aisle → Rack → Shelf → Bin) كسجلات `Warehouse` متداخلة عبر `ParentId`، مع خصائص لكل مستوى.
- محرك تخصيص Bins حقيقي يعمل عند الإدخال والإخراج، مع تقسيم سطر المستند إلى عدة أسطر Bin، وتوزيع السيريالات/الدفعات عليها.
- مستندات: GRPO، Goods Receipt، Goods Issue، Inventory Transfer، Pick List، PO، Delivery، Returns.
- Serial + Batch مع تتبّع Bin لكل منهما.

**الفجوات الحرجة الثلاث (بالترتيب):**

| # | الفجوة | الأثر |
|---|---|---|
| 1 | **استراتيجيات Putaway/Removal غير مُنفَّذة إطلاقًا** — `WmsZonePutawayStrategy` و`WmsZoneRemovalStrategy` معرَّفتان كـenum + حقل + DTO، و**لا يقرأهما أي سطر منطق في المشروع كله**. الاختيار يتم بترتيب اجتياز الشجرة عشوائيًا. | لا FIFO/FEFO/LIFO، لا Directed Putaway. هذا **جوهر WMS** وهو غائب. |
| 2 | **الـBin الذي يمسحه الموظف يُلغى ويُستبدل بجذر المستودع** — `GoodsReceiptPOService.cs:400-420` و`ApprovedReturnRequestService.cs:385-405` تستدعيان `GetRootWarehouseIdAsync` فتُهمل الـBin المُختار، ثم يُخصَّص Bin تلقائيًا. كما أن `BinLocationId` **غير موجود أصلًا على `DocumentLine`** (موجود فقط على `InventoryTransactionLine` و`InventoryMovement`). | متطلّب الـSRS "Scan bin/location" **غير قابل للتنفيذ حاليًا** على مستوى المستند. |
| 3 | **الجرد (Counting) غير موجود بالكامل** — صفر نتائج للبحث عن `CycleCount` / `InventoryCount` / `StockCounting` في المشروع كله. | البند 3.4.2 كامل غير مُنفَّذ، ولا يوجد مستند Inventory Counting ولا Inventory Posting. |

---

## 2. تدقيق البنود — القسم 1 و 2 (المقدمة والنظرة العامة)

### 1.1 المبادئ الأساسية

| البند في SRS | الحالة | الدليل من الكود | المعالجة وفق SAP B1 |
|---|---|---|---|
| Real-time operations standalone + ERP sync اختياري | ⚠️ | لا يوجد وضع Standalone منفصل؛ النظام **هو** الـERP نفسه (قاعدة بيانات واحدة). لا توجد طبقة مزامنة خارجية. | **قرار معماري مطلوب:** SAP B1 لا يعمل "standalone WMS"؛ الـWMS في عالم SAP إما داخل B1 (Bin Locations) أو add-on مثل Produmex بقاعدة بيانات مشتركة. **التوصية:** حذف مفهوم "Standalone + Sync" من الـSRS واعتماد النموذج الحالي (وحدة داخل نفس القاعدة) — أبسط وأدق وأقل خطرًا. |
| Role-based access (Manager web / Employee mobile) | 🚫 | خارج النطاق بطلبك | — |
| Barcode/SSCC/Serial/Batch | ⚠️ | Barcode: ✅ `Product.Barcode` + `Warehouse.Barcode`. Serial: ✅ `SerialNumber`. Batch: ✅ `BatchNumber` + `BatchNumberStock`. **SSCC: ❌ صفر كود.** | راجع بند 3.3.4 أدناه — SSCC ليس ميزة SAP B1 أساسية. |
| Configurable features | ✅ | `Configuration` + `ConfigurationLine` + `OrgConfiguration` موجودة ومستخدمة (مثال حقيقي: `GiftMaxPercentageFromOrder`). | مطابق لمنهج SAP B1 (Document Settings / General Settings). |
| Audit trails | ⚠️ | `Log.cs` + `ApprovalHistory.cs` موجودان، لكن لا يوجد سجل تدقيق موحّد للعمليات المخزنية بمستوى (من/متى/أي Bin/أي مسح باركود). | SAP B1 يوفّر `Change Log` لكل مستند + `Inventory Audit Report`. **التوصية:** الاعتماد على `InventoryMovement` (موجود، ويحمل `BinLocationId`) كسجل الحركة الرسمي، وإضافة حقول (UserId, DeviceId, ScanTimestamp) عليه بدل إنشاء جدول `wms_audit` جديد. |
| Bins as sub-units within zones | ✅ | `WmsLevelType` = Warehouse/Zone/Aisle/Rack/Shelf/Bin + شجرة `Warehouse.ParentId`. | **أدق من SAP B1 نفسه** (B1 يحدّد 4 مستويات فرعية فقط عبر Bin Location Field Activation). لا تغيير مطلوب. |
| Mobile offline + camera scan | ❌ | لا يوجد أي دعم Offline/Queue/Sync في الـBackend. | راجع بند 3.1/8 أدناه. |

### 2.2 الميزات الأساسية

| البند | الحالة | الدليل | المعالجة وفق SAP B1 |
|---|---|---|---|
| Multi-warehouse assignment per employee | 🚫/⚠️ | جزئي: `Employee` موجود، ولا يوجد جدول ربط Employee↔Warehouse. | (صلاحيات — خارج النطاق، لكن **نموذج البيانات** ناقص فعلًا لو احتجته لاحقًا لفلترة العمليات وليس للصلاحية.) |
| PO filtering by assigned warehouses | ⚠️ | يعتمد على البند أعلاه — غير موجود. | SAP B1: يتم عبر `Data Ownership` + `Authorization`. مؤجَّل. |
| **Locking mechanism (منع GRPO متزامن على نفس PO)** | ❌ | صفر كود. نتائج `LockedBy` كلها من وحدة Helpdesk فقط. | **SAP B1 لا يقفل المستندات مسبقًا** — يستخدم **Optimistic Concurrency**: عند الحفظ يفشل ويظهر "Document was modified by another user". **والمشروع لديك يملك هذا فعلًا**: حقل `Version` (Guid) موجود على كل مستند. **التوصية القوية: احذف مفهوم الـLock من الـSRS واستبدله بالتحقق من `Version` عند الحفظ** — هذا هو نمط المؤلف ونمط SAP معًا، وبلا جداول جديدة ولا مشكلة "أقفال معلّقة". |
| Attachments/Notes per document | ⚠️ | `Asset` + `AssetCollection` موجودة، وحقل `assets` يظهر فعليًا في `OrderDto`. لكن لم أجد ربطًا صريحًا لمرفقات على مستوى **السطر**. | SAP B1: `Attachments` tab على الترويسة فقط (لا مرفقات لكل سطر). **التوصية:** طابق SAP — مرفقات على مستوى المستند فقط، واحذف "attachments per item" من الـSRS. |
| Bin-level directed putaway/picking | ❌ | راجع الفجوة #1 في الخلاصة. | راجع بند 3.2.1 أدناه (المعالجة التفصيلية). |
| Bin capacity tracking/alerts | ⚠️ | التتبّع: ✅ (`WarehouseService.GetBinCapacityAsync` تحسب `CurrentOccupied`/`RemainingCapacity`/`OccupiedPercentage`). الفرض عند الإدخال: ✅ (`InventoryTransactionHelper` يرمي "Insufficient bin capacity"). **التنبيهات (`BinCapacityThresholdPercent`): ❌ غير مستخدمة في أي منطق.** كما أن **`MaxWeight` و`MaxVolume` مُعرَّفان على `WmsBinAttribute` ولا يُفحصان إطلاقًا.** | SAP B1 Bin Location Master يملك `Maximum Quantity` و`Maximum Weight` ويفرضهما عند التخصيص. **التوصية:** توسيع الفحص القائم في `InventoryTransactionHelper` ليشمل الوزن/الحجم. **مؤكَّد بالفحص: كل البيانات اللازمة موجودة على `Product`** (`Weight`, `Length`, `Width`, `Height` كلها `decimal` مع وحدات قياس مرتبطة) — فالحجم = الطول×العرض×الارتفاع، ولا حاجة لأي حقل جديد. |

---

## 3. تدقيق البنود — القسم 3.1 (تدفقات مشتركة)

| البند | الحالة | الدليل / المعالجة وفق SAP |
|---|---|---|
| 1. Login/Auth | 🚫 | خارج النطاق. |
| 2. Dashboard (قوائم مفلترة) | ⚠️ | الخدمات كلها تدعم `QueryPayload` + فلاتر (`Select`)، فالبنية جاهزة. الفلترة حسب مستودعات الموظف تحتاج جدول الربط الناقص. |
| 3. PO/Operation Selection | ✅ | `PurchaseOrderService` + `GoodsReceiptPOService` مع `Copy From` عبر `DocumentHelper`/`BaseDocumentId`. مطابق لمنهج SAP (Draw Document Wizard). |
| 4. Item Processing — إدخال كمية | ✅ | مدعوم عبر `DocumentLine.Quantity` + `OpenQuantity`. |
| 4. Item Processing — **مسح الباركود** | ⚠️ | `Product.Barcode` موجود، لكن **لا يوجد Endpoint للبحث بالباركود** (لم أجد `GetByBarcode`). العميل المحمول يحتاجه. | **التوصية:** إضافة `ProductService.GetByBarcodeAsync(string barcode)` — بسيطة وبلا مخاطر، مطابقة لسلوك SAP B1 (حقل `BcdCode` ومسحه يملأ السطر). |
| 4. Serials | ✅ | `SerialNumber` + `SerialNumberService` + تحقق من التفرّد. |
| 4. Batch (Expiry/Lot) | ✅ | `BatchNumber` يحمل **`ExpiryDate` + `ManufacturingDate` + `BatchStatus`** ✅، و`BatchNumberStock` يربط الدفعة بـ(المنتج، المستودع/الـBin، الكمية). **نتيجة مهمة: كل بيانات FEFO متوفرة بالفعل — ينقص منطق الترتيب فقط.** ⚠️ لكن `BatchNumberStock.Quantity` من نوع **`double`** بدل `decimal` (خطر دقة عائمة). |
| 4. SSCC | ❌ | راجع 3.3.4. |
| 4. **مسح الـBin** | ❌ | **الفجوة #2**: لا يوجد `BinLocationId` على `DocumentLine`، والـBin المُختار يُستبدل بالجذر. |
| 4. Non-PO item | ⚠️ | ممكن تقنيًا (إضافة سطر بلا `BaseDocumentLineId`)، لكن لا يوجد **حقل سبب (Reason)** ولا Config للسماح/المنع. | SAP B1: يسمح بإضافة أسطر جديدة في GRPO غير مرتبطة بالـPO افتراضيًا. **التوصية:** إضافة `Configuration` بمفتاح `AllowNonPoItemsInGRPO` + حقل `Remarks` على السطر (موجود؟ 🔍) بدل جدول أسباب جديد. |
| 5. Validation: Qty ≤ PO qty | ⚠️ | `OpenQuantity` موجود ويُدار، لكن **لا يوجد فحص صريح لتجاوز الكمية ولا Tolerance** في `GoodsReceiptPOService`. | SAP B1 يسمح بالاستلام الزائد افتراضيًا ويُغلق السطر. **التوصية:** فحص صريح + `Configuration` للتسامح (وليس `OverReceiptTolerancePercent` الموجود على الـBin — فذاك يخص سعة الـBin لا كمية الـPO). |
| 5. Serial uniqueness (item+serial) | ✅ | مُنفَّذ في `SerialNumberService`. |
| 6. Save/Submit + Auto-lock | ⚠️ | الحفظ ✅ / القفل ❌ (استبدله بـ`Version` كما في 2.2). |
| 7. Conflicts / force-take | ❌ | يسقط تلقائيًا مع اعتماد Optimistic Concurrency. **التوصية: احذفه من الـSRS.** |
| 8. **Offline queue + sync** | ❌ | صفر دعم في الـBackend. | SAP B1 نفسه لا يوفّر offline؛ Produmex WMS يوفّره عبر تطبيق مخصّص. **التوصية:** اعتبره **مسؤولية تطبيق Flutter** (طابور محلي + إعادة إرسال)، ويُشترط على الـBackend فقط: **Idempotency Key** لكل عملية لمنع الازدواج عند إعادة الإرسال. هذه إضافة صغيرة وحاسمة. |
| 9. Attachments | ⚠️ | راجع 2.2. |
| 10. Notifications | ✅ | بنية كاملة: `Message`, `MessageTemplate`, `MessagingChannel`, `MessageRecipient`, `MessageEvent`. |

### أفضل الممارسات المذكورة في الـSRS

| البند | الحالة | المعالجة |
|---|---|---|
| GS1 standards for barcodes/SSCC | ❌ | لا يوجد تحقق GS1. **التوصية:** مؤجَّل لما بعد إنجاز اللوجيك الأساسي. |
| Real-time inventory / prevent oversell | ✅ | `ProductAvailability` (OnHand/Committed/Ordered) + `UpdateCommittedValue` + فحص النقص في `InventoryTransactionHelper` ("Insufficient stock across bins"). مطابق لمنهج SAP (OnHand/Committed/Ordered = InStock/IsCommited/OnOrder). |
| Audit كل مسح | ⚠️ | راجع 1.1. |
| **Bin optimization (FIFO/ABC)** | ❌ | **الفجوة #1.** ABC غير موجود إطلاقًا (لا حقل تصنيف ABC على `Product`). |

---

## 4. تدقيق البنود — القسم 3.2 (العمليات الواردة)

### 3.2.1 GRPO — الحالة: ⚠️ موجود بقوة مع فجوتين

**الموجود فعليًا:**
- `GoodsReceiptPOService` + `GoodsReceiptPO/Line/Address/Expense/SpecialLine` كاملة.
- ربط بالـPO عبر `BaseDocumentId`/`BaseDocumentLineId` + إدارة `OpenQuantity`.
- تحويل تلقائي إلى حركة مخزنية: `CommerceToInventoryHelper.ConvertToGoodsReceiptTransaction` → `GoodsReceiptService.Add` ([GoodsReceiptPOService.cs:428-440](../21-07-2026-claude/ITM.Core.Commerce.Backend/ITM.Core.Commerce/Services/GoodsReceiptPOService.cs)).
- تخصيص Bins تلقائي مع **فرض السعة** ([InventoryTransactionHelper.cs:423-450](../21-07-2026-claude/ITM.Core.Products.Backend/ITM.Core.Products/Helper/InventoryTransactionHelper.cs)).
- حل المستودع جغرافيًا عند غيابه (`GetWarehouseIdByCoordinatesAsync`) — ميزة إضافية غير مذكورة في الـSRS.

**الناقص:**

| الفجوة | التفصيل | المعالجة وفق SAP B1 |
|---|---|---|
| مسح/اختيار الـBin | `line.WarehouseId` يُستبدل بالجذر ([:400-420](../21-07-2026-claude/ITM.Core.Commerce.Backend/ITM.Core.Commerce/Services/GoodsReceiptPOService.cs)) ولا يوجد `BinLocationId` على `DocumentLine`. | **SAP B1:** نافذة `Bin Location Allocation` تفتح لكل سطر في مستودع Bin-Managed، وتسمح بالتوزيع اليدوي **أو** بالتخصيص التلقائي. **التوصية الدقيقة:** (1) إضافة `Guid? BinLocationId` على `DocumentLine` — اختياري، (2) في `InventoryTransactionHelper`: إن كان مُعبّأ → احترمه وتحقّق فقط من السعة؛ إن كان فارغًا → استخدم التخصيص التلقائي الحالي. **هذا يحافظ على كل السلوك القائم ولا يكسر شيئًا.** |
| Over-Receipt tolerance (A4) | لا فحص. | `Configuration`: `GRPO.OverReceiptTolerancePercent` + رمي استثناء أو `ApprovalStatus.Pending` (البنية موجودة: `ApprovalConfiguration` + `ApprovalHistory`). |
| Draft/Partial (A1) | ✅ متاح | **مؤكَّد:** `DocumentStatus` يحتوي على `Draft` صراحةً (إلى جانب Open/Close/Cancel/PartialOpen/PendingPayment)، ومستند `DraftDocument` منفصل موجود مع خدمته. **لا حاجة لأي بنية جديدة** — يكفي تمرير الحالة. مطابق لمنهج SAP B1 (`Document Drafts`). |
| Reason على السطر (A2) | لا حقل سبب. | حقل نصّي + قائمة عبر `Configuration` (SAP B1 يستخدم `Remarks`/UDF). |

### 3.2.2 Sales Return — الحالة: ✅ قوي جدًا (أقوى من الـSRS)

- `ReturnRequest` → `ApprovedReturnRequest` → `CreditNote` **مع توليد تلقائي لإشعار الدائن** (`TryAutoGenerateCreditNoteAsync`) وتتبّع سلسلة المستندات (`DocumentRelationMapService`) — يتجاوز متطلّب "A4 Credit Note".
- تخصيص Bins عند الإرجاع مُنفَّذ (`ApprovedReturnRequestService.cs:385-405`).
- **الناقص:** ❌ حقل **Reason Code إلزامي** لكل سطر إرجاع (SRS يذكره صراحة "mandatory: damage/defect/wrong-item")، و❌ التوجيه التلقائي إلى **Quarantine** (A2).
- **معالجة SAP (مؤكَّدة بالفحص):** `Warehouse.DamagedGoodsWarehouseId` و`InspectionWarehouseId` **موجودان على النموذج ومُهيّآن في بيانات الـSeed**، لكن البحث الشامل أثبت أنهما **يُستخدمان فقط في CRUD والتحقق داخل `WarehouseService` (الأسطر 271-275, 556-557) — وصفر منطق أعمال يستخدمهما للتوجيه.** أي أن البنية جاهزة بالكامل والمنطق وحده غائب. **التوصية:** توجيه سطر الإرجاع إلى `DamagedGoodsWarehouseId` عند سبب "تالف" وإلى `InspectionWarehouseId` عند "يحتاج فحصًا" — تعديل صغير في `ApprovedReturnRequestService` عند تحديد `line.WarehouseId`، بلا أي حقول جديدة.

### 3.2.3 Goods Receipt (عام) — الحالة: ✅

- `GoodsReceipt` + `GoodsReceiptLine` + `...LineBatchNumbers` + `...LineSerialNumbers` + `GoodsReceiptService`.
- مطابق تمامًا لمستند SAP B1 `Goods Receipt` (زيادة مخزون بلا PO).

### 3.2.4 Receipt from Production — الحالة: ❌ غير موجود

- لا يوجد `ProductionOrder` ولا `ReceiptFromProduction` (رغم وجود `BillOfMaterial` + `PlanningProposal` + `PlanningTask`).
- **معالجة SAP:** SAP B1 لديه `Production Order` → `Receipt from Production` + `Issue for Production`.
- **التوصية:** **استبعده من المرحلة الحالية** — يتطلب وحدة إنتاج كاملة. أو (حل مؤقت مطابق لـSAP): استخدام `Goods Receipt` العام مع `BaseDocumentType` يشير لأمر الإنتاج لاحقًا.

---

## 5. تدقيق البنود — القسم 3.3 (العمليات الصادرة)

### 3.3.1 Delivery — الحالة: ⚠️

- `Delivery` + `DeliveryLine` + **`DeliveryLineWarehouse`** ✅ (يدعم توزيع السطر على عدة مستودعات/Bins).
- الخصم من المخزون: ✅ عبر `UpdateWerehouse` + `GoodsIssues`.
- **الناقص:** مرحلة `Pick` منفصلة عن `Pack` (الـSRS يطلب Pick ثم Pack ثم Issue). حاليًا `PickList` منفصل تمامًا عن `Delivery` بلا ربط حالة.
- **معالجة SAP:** `Pick and Pack Manager` في SAP B1 يربط: `Sales Order` → `Pick List` (Open→Released→Picked) → `Delivery`. **التوصية:** إضافة `PickListStatus` transitions + ربط `PickList.BaseDocument` بالـDelivery (`PickListLine.BaseDocumentId/Type` **موجودان بالفعل** ✅) — ينقص فقط منطق الانتقال والتحقق.

### 3.3.2 Goods Return PO — الحالة: ✅
- `GoodsReturnPO` + خدماته كاملة، مطابق لمستند SAP B1 `Goods Return`.

### 3.3.3 Goods Issue — الحالة: ✅
- `GoodsIssues` + `GoodsIssuesLine` + Batch/Serial + خدمة كاملة + تخصيص Bins عكسي (يستخرج الـBin من موقع السيريال/الدفعة فعليًا). مطابق لـSAP B1 `Goods Issue`.

### 3.3.4 Pick and Pack — الحالة: ⚠️ (Pick) / ❌ (Pack + SSCC)

| البند | الحالة | التفصيل |
|---|---|---|
| Pick List | ⚠️ | `PickList` + `PickListLine` + `PickListService` موجودة، وتحمل `WarehouseId` (قابل لأن يكون Bin). **عيبان حقيقيان:** (1) `QuantityToPick`/`QuantityPicked` من نوع **`int`** بينما كل كميات النظام `decimal(18,2)` — **يكسر المنتجات الكسرية (كجم/لتر)**. (2) لا ترتيب مسار (Route/Sequence). |
| Pack / Package | ❌ | لا يوجد أي كيان Package/Carton/Pallet. |
| **SSCC** | ❌ | صفر كود في المشروع كله. |

**معالجة SAP الدقيقة والصادقة:**
- **SSCC ليس ميزة في SAP Business One الأساسي.** هو ميزة في `Produmex WMS` (add-on معتمد من SAP). لذلك: **إمّا** تعتبره خارج نطاق النسخة الحالية (توصيتي)، **وإمّا** تبنيه ككيان جديد `HandlingUnit` (Id, SSCC18, ParentHandlingUnitId, Status, Contents) — وهو مشروع مستقل بحجمه.
- **Packages في SAP B1 الأساسي:** يوجد تبويب `Packages` داخل مستند الـDelivery (Package Type + محتويات + وزن). **التوصية العملية:** كيان بسيط `DeliveryPackage` مرتبط بالـDelivery بدل نظام SSCC كامل.
- **إصلاح فوري ومنخفض المخاطر:** تحويل `QuantityToPick`/`QuantityPicked` إلى `decimal(18,2)` لمطابقة بقية النظام.

---

## 6. تدقيق البنود — القسم 3.4 (عمليات أخرى)

### 3.4.1 Inventory Transfer — الحالة: ⚠️

**الموجود:** `InventoryTransfer` + `InventoryTransferLine` (From/To على الترويسة **وعلى السطر** ✅) + `InventoryTransferLineSerialNumber` + `InventoryTransferService`.

**الناقص (مؤكَّد بالمقارنة البنيوية):**
1. ❌ **لا يوجد `InventoryTransferLineBatchNumber`** — بينما `GoodsReceiptLine` و`GoodsIssuesLine` كلاهما يملك مجموعة دفعات. **النتيجة: لا يمكن نقل صنف مُدار بالدفعات مع تتبّع الدفعة.** هذه فجوة بنيوية حقيقية وغير متسقة.
2. ❌ لا يوجد حقل `Status` على `InventoryTransfer` (لا Draft ولا Posted) بينما بقية المستندات تملكه.
3. ❌ لا يوجد `Inventory Transfer Request` (طلب نقل) — موجود في SAP B1 كمستند سابق للنقل.

**معالجة SAP:** SAP B1: `Inventory Transfer Request` → `Inventory Transfer`، وكلاهما يدعم Batch/Serial + Bin Allocation في الطرفين (From Bin / To Bin).
**التوصية:** إضافة `InventoryTransferLineBatchNumber` (نسخ حرفي لنمط `GoodsIssuesLineBatchNumbers`) — **هذا أعلى بند أولوية/عائد في هذا القسم** لأنه اتساق بنيوي بحت وبلا منطق جديد.

### 3.4.2 Counting (Cycle Count / Full Inventory) — الحالة: ❌ غير موجود إطلاقًا

صفر نتائج بحث. لا مستند، لا خدمة، لا نموذج.

**معالجة SAP B1 (المرجع الرسمي):**
- SAP B1 يملك **مستندَين منفصلين**: `Inventory Counting` (تسجيل العدّ فقط، بلا أثر مخزني) ثم `Inventory Posting` (ترحيل الفروقات فعليًا).
- يدعم: Blind Count (إخفاء الكمية المتوقعة)، عدّة عدّادين لنفس الموقع، حالة `Counted`/`Closed`، و`Cycle Count Determination` (تحديد دورية العد لكل صنف/مستودع).
- **التوصية البنيوية المطابقة:**
  - `InventoryCounting` + `InventoryCountingLine` (ProductId, WarehouseId, **BinLocationId**, CountedQuantity, ExpectedQuantity nullable للـBlind, Variance محسوبة, CounterEmployeeId, Status).
  - `InventoryPosting` + خطوطه → يولّد `InventoryTransaction` بفارق موجب/سالب (يمكن إعادة استخدام `GoodsReceiptService`/`GoodsIssuesService` الموجودتين للترحيل بدل محرك جديد).
  - **لا تدمج العدّ بالترحيل في مستند واحد** — الفصل هو جوهر منهج SAP وهو ما يسمح بالمراجعة والاعتماد.

### 3.4.3 Pick List — راجع 3.3.4.

### 3.4.4 Package — راجع 3.3.4 (❌).

---

## 7. القسم 3.5 (لوحة المدير) و4 و5

| البند | الحالة | ملاحظة |
|---|---|---|
| KPIs/Reports/Export | ✅ | وحدة `Reporting` كاملة: `ReportDefinition`, `ReportSchedule`, `ReportExecutionLog`, `ReportOutputFile`, `ReportDashboard`, `ReportDashboardWidget`. تغطي متطلّبات الـSRS بالكامل وأكثر. |
| Operations Queue (drafts/locks) | ⚠️ | Drafts ✅ / Locks ❌ (محذوف حسب توصيتنا). |
| Configs (assignments/tolerances) | ⚠️ | البنية موجودة، القيم ناقصة. |
| Performance/Security/Reliability | 🚫 | غير قابلة للتدقيق بقراءة كود ثابتة. |
| **القسم 5: Data Model Extensions** | ❌ **يجب حذفه** | الـSRS يقترح `wms_grpo`, `wms_lock`, `wms_audit` وحقول `sscc`, `serials (JSON array)`. **هذا يتعارض جذريًا مع نمط المؤلف القائم**: (1) `GoodsReceiptPO` موجود كمستند كامل ولا حاجة لـ`wms_grpo`، (2) الأقفال مرفوضة لصالح `Version`، (3) **تخزين السيريالات كـJSON array يخالف التصميم الحالي** الذي يستخدم جداول علاقات صريحة (`GoodsReceiptLineSerialNumbers`) — وهو الأصح للفهرسة والتتبّع. **التوصية: احذف القسم 5 بالكامل واستبدله بمرجع للنماذج القائمة.** |

---

## 8. سجل الفجوات مرتّبًا حسب الأولوية (خطة التنفيذ المقترحة)

| # | الفجوة | الحجم | المخاطرة على القائم | الأولوية |
|---|---|---|---|---|
| 1 | `InventoryTransferLineBatchNumber` (اتساق بنيوي — نسخ نمط `GoodsIssuesLineBatchNumbers`) | صغير | لا شيء (إضافة بحتة) | 🔴 فورية |
| 2 | `PickListLine.QuantityToPick/QuantityPicked` من `int` → `decimal(18,2)` | صغير | منخفضة (Migration) | 🔴 فورية |
| 3 | `BatchNumberStock.Quantity` من **`double` → `decimal(18,2)`** — النوع الحالي عائم ثنائي ويُنتج انحرافات دقة في كميات الدفعات، وهو **غير متسق مع كل كميات النظام** | صغير | منخفضة (Migration) | 🔴 فورية |
| 4 | `ProductService.GetByBarcodeAsync` (لتمكين المسح من المحمول) | صغير | لا شيء | 🔴 فورية |
| 5 | `DocumentLine.BinLocationId` (اختياري) + احترامه في `InventoryTransactionHelper` | متوسط | **منخفضة** إن بقي التخصيص التلقائي هو السلوك الافتراضي عند الفراغ | 🔴 عالية |
| 6 | تفعيل `PutawayStrategy`/`RemovalStrategy` في محرك التخصيص (**ترتيب قائمة `bins` قبل الحلقة فقط**) — كل بيانات FEFO متوفرة (`BatchNumber.ExpiryDate`) | متوسط | منخفضة (نقطة تغيير واحدة معزولة) | 🔴 عالية |
| 7 | Reason Code + توجيه Quarantine للمرتجعات (الحقول `DamagedGoodsWarehouseId`/`InspectionWarehouseId` جاهزة وغير مستخدمة) | متوسط | منخفضة | 🟠 متوسطة |
| 8 | Over-Receipt tolerance في GRPO | صغير | منخفضة | 🟠 متوسطة |
| 9 | Idempotency Key لدعم Offline | متوسط | منخفضة | 🟠 متوسطة |
| 10 | فرض `MaxWeight`/`MaxVolume` (بيانات `Product` جاهزة) + تنبيه `BinCapacityThresholdPercent` | متوسط | منخفضة | 🟠 متوسطة |
| 11 | **وحدة الجرد** (`InventoryCounting` + `InventoryPosting`) | **كبير** | لا شيء (وحدة جديدة) | 🟡 مرحلة تالية |
| 12 | ربط PickList↔Delivery بحالات Pick/Pack | متوسط | متوسطة | 🟡 مرحلة تالية |
| 13 | Packages (بديل SSCC المبسّط) | كبير | لا شيء | 🟡 مرحلة تالية |
| 14 | SSCC/GS1 كامل + Production Orders | كبير جدًا | — | ⚪ خارج النطاق المقترح |

---

## 9. نتائج التحقق النهائي للبنود التي كانت معلّقة

| السؤال | النتيجة المؤكَّدة بالفحص |
|---|---|
| هل `BatchNumber` يحمل تاريخ صلاحية؟ | ✅ **نعم** — `ExpiryDate` + `ManufacturingDate` + `BatchStatus` كلها موجودة. **الأثر: FEFO قابل للتنفيذ فورًا، ينقصه الترتيب فقط.** |
| هل حالة Draft مدعومة؟ | ✅ **نعم** — `DocumentStatus.Draft` موجودة في الـenum، ومستند `DraftDocument` + خدمته موجودان. |
| هل يوجد ربط Employee ↔ Warehouse؟ | ❌ **لا** — `Employee.cs` لا يحمل أي `WarehouseId` ولا يوجد جدول ربط. (يلزم فقط إن أردت **فلترة عمليات** حسب مستودع الموظف؛ أما الصلاحيات فخارج النطاق.) |
| هل `DamagedGoods`/`Inspection` warehouses مستخدمان؟ | ❌ **لا** — CRUD وSeed فقط، صفر منطق أعمال. |
| هل بيانات الوزن/الأبعاد متوفرة لفرض سعة الـBin؟ | ✅ **نعم** — `Product.Weight/Length/Width/Height` كلها `decimal` مع وحدات مرتبطة. |

### الوحيد المتبقي (🔍) — يحتاج مرجعًا منك من SAP B1 10.0

لتثبيت تفاصيل البنود 5 و6 و11 بدقة نهائية بدل الاعتماد على وصف عام، أحتاج لقطات من حسابك لـ:
1. `Bin Location Master Data` — كامل الحقول والتبويبات (خصوصًا Restrictions وAllow Auto Allocation وBin Ranking).
2. شاشة `Bin Location Allocation` أثناء إنشاء GRPO في مستودع Bin-Managed (لضبط سلوك البند 5 حرفيًا).
3. مستند `Inventory Counting` + `Inventory Posting` (لضبط بنية وحدة الجرد في البند 11).

---

## 10. توصية ختامية بشأن الـSRS نفسه

الـSRS الحالي كُتب كأنه **نظام WMS مستقل جديد**، بينما الواقع أنك تملك **ERP كامل ببنية WMS متقدمة أصلًا**. أوصي بإعادة صياغته على أساس:
- حذف: Standalone/Sync، Locking، `wms_*` tables، serials-as-JSON، attachments per line.
- تثبيت: الشجرة الحالية، محرك التخصيص الحالي، مستندات SAP القائمة (GRPO/GR/GI/Transfer/PickList).
- التركيز على الفجوات 1-9 أعلاه أولًا — فهي تُغلق **معظم** الـSRS بجهد صغير نسبيًا وبمخاطرة شبه معدومة على المنطق القائم.
