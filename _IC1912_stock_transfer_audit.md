# IC-1912 — Stock Transfer: تدقيق سطرًا بسطر للدعم الحالي مقابل ما بنيناه في GR & GI

> **المرجع:** `InventoryTransferService` + `InventoryTransactionHelper` + `GoodsIssuesService` + `GoodsReceiptService`
> **تاريخ التدقيق:** 2026-09-09
> **المنهج:** كل بند مبني على قراءة كود فعلية مع مرجع ملف/سطر — لا افتراضات. **لم يُطبَّق أي تعديل.**

---

## 1. الخلاصة التنفيذية

**البنية الأساسية ذكية ومبنية على نمط المؤلف الصحيح:** `InventoryTransfer` **لا يُعيد اختراع منطق المخزون** — بل يُولِّد `GoodsIssues` (من المصدر) + `GoodsReceipt` (إلى الهدف)، فيرث تلقائيًا **كل** ما بنيناه في `InventoryTransactionHelper` (تخصيص Bins، فحص السعة، فحص النقص، سجل الحركة).

**لكن الميراث ناقص في ثلاث نقاط قاتلة:**

| # | الفجوة | الأثر |
|---|---|---|
| 1 | **الدفعات (Batch) غير مدعومة إطلاقًا** — لا يوجد `InventoryTransferLineBatchNumber`، ولا يُمرَّر `BatchNumbers` في `DatabaseAdd` | **نصف أنواع المواد (Batch، Batch+Serial) لا يمكن نقلها مطلقًا** — ترمي استثناءً |
| 2 | **الـBin الهدف يُلغى ويُعاد تخصيصه تلقائيًا** — `HandleBinManagedLinesAsync` تُطبِّع `ToWarehouseId` إلى الجذر ثم تُخصِّص Bin بالسعة | **النقل Bin→Bin قد يفشل أو يصبح بلا أثر** (قد يُعيد البضاعة لنفس الـBin المصدر) |
| 3 | **لا Reason، لا Status، لا Approval، ولا ربط بسجل الحركة** | **3 من متطلبات التاسك غير مُنفَّذة إطلاقًا** |

---

## 2. مصفوفة الدعم — أنواع المواد (مُتحقَّق منها من منطق التحقق الفعلي)

مرجع التحقق: [`InventoryTransactionHelper.UpdateInventoryTransactionLine`](../21-07-2026-claude/ITM.Core.Products.Backend/ITM.Core.Products/Helper/InventoryTransactionHelper.cs) الأسطر 88-222.

| نوع المادة | جانب GI (المصدر) | جانب GR (الهدف) | النتيجة النهائية |
|---|---|---|---|
| **Normal** | ✅ يمرّ عبر `else` النهائي بلا سيريال/دفعة | ✅ نفس الشيء | ✅ **يعمل** |
| **SerialNumber** | ✅ `SerialNumbersIds` تُمرَّر فعليًا من `InventoryTransferLineSerialNumbers` | ✅ `CheckSerialNumbers` تنقل السيريال: `serialNumber.WarehouseId = warehouse.Id; Status = Active` | ✅ **يعمل** (مع تحفّظ الـBin أدناه) |
| **BatchNumber** | ❌ `if (!hasBatchNumbersIds && !hasBatchNumbers) throw "Batch numbers is null"` | ❌ `if ((int)batchNumberTotalQuantity != line.InventoryQuantity) throw "The batch number quantity does not match..."` (0 ≠ الكمية) | ❌ **محظور تمامًا** |
| **BatchAndSerialNumber** | ❌ يتطلب السيريال **و** الدفعة معًا → الدفعة مفقودة → يرمي | ❌ نفس الشيء | ❌ **محظور تمامًا** |

**السبب الجذري الوحيد للحظر:** في [`InventoryTransferService.DatabaseAdd:131-149`](../21-07-2026-claude/ITM.Core.Products.Backend/ITM.Core.Products/Services/InventoryTransferService.cs:131):
```csharp
var goodsIssuedLineDto = new GoodsIssuesLineDto
{
    ProductId = line.ProductId,
    WarehouseId = (Guid)line.FromWarehouseId,
    UnitOfMeasurementQuantity = line.UnitOfMeasurementQuantity,
    UnitOfMeasurementId = line.UnitOfMeasurementId,
    QuantityPerUnit = line.QuantityPerUnit,
    SerialNumbersIds = line.InventoryTransferLineSerialNumbers.Select(x => x.SerialNumberId).ToList(),
    // ⚠️ لا يوجد BatchNumbers إطلاقًا — والكيان نفسه لا يملك مجموعة دفعات أصلًا
};
```
مقارنة بنيوية حاسمة: `GoodsReceiptLine` و`GoodsIssuesLine` **كلاهما** يملك `...LineBatchNumbers`، بينما `InventoryTransferLine` يملك `InventoryTransferLineSerialNumbers` **فقط**. **عدم اتساق بنيوي صريح.**

---

## 3. مصفوفة الدعم — تركيبات المستودعات

مرجع: [`HandleBinManagedLinesAsync`](../21-07-2026-claude/ITM.Core.Products.Backend/ITM.Core.Products/Helper/InventoryTransactionHelper.cs) الأسطر 224-455.

| الاتجاه | السلوك الفعلي | الحكم |
|---|---|---|
| **عادي → عادي** | يمرّ كـ`passthroughLines`؛ الخصم/الإضافة عبر `SaveInventoryMovement` مع فحص `OnHand >= InventoryQuantity` | ✅ **يعمل بالكامل** |
| **عادي → Bin Managed** | GI عادي؛ GR: يُطبَّع الهدف للجذر ثم يُخصَّص Bin **تلقائيًا حسب السعة المتبقية** | ⚠️ **يعمل، لكن الـBin الذي اختاره المستخدم يُهمَل تمامًا** |
| **Bin Managed → عادي** | GI: يُستخرَج الـBin المصدر من موقع السيريال/الدفعة أو من `onHandByBin`؛ GR عادي | ⚠️ **يعمل، لكن الـBin المصدر لا يُحترَم إن حدَّده المستخدم يدويًا** |
| **Bin → Bin (نفس الجذر)** | ⚠️ **خطر حقيقي:** كلا الطرفين يُطبَّعان لنفس الجذر. GR يُخصِّص Bin بالسعة — **وقد يختار نفس الـBin المصدر** | ❌ **للسيريال: يرمي `"serial number ... already exists in this warehouse"`** (لأن `CheckSerialNumbers` في GR يرفض إن كان `serialNumber.WarehouseId == LocationId`).<br>**للعادي: قد يصبح النقل بلا أثر فعلي** (خصم من BinA ثم إضافة إلى BinA) |
| **Bin → Bin (جذرين مختلفين)** | نفس منطق "عادي → Bin" لكن في الطرفين | ⚠️ **يعمل، لكن الوجهة الدقيقة مُهمَلة** |

### السبب البنيوي للفجوة #2
```csharp
// HandleBinManagedLinesAsync — يُطبِّع أي Bin مُحدَّد إلى الجذر
if (rootByOriginalId.TryGetValue(line.WarehouseId, out var rootId) && rootId != line.WarehouseId)
    line.WarehouseId = rootId;      // ← اختيار المستخدم يُمحى هنا
```
ثم يُخصَّص Bin تلقائيًا:
```csharp
foreach (var bin in bins) {                       // ترتيب اجتياز الشجرة، بلا استراتيجية
    available = (GI) ? onHandByBin[bin] : bin.RemainingCapacity;
    ...
}
```
**النتيجة:** لا يوجد أي حقل `BinLocationId` على `InventoryTransferLine` أصلًا، ولا مسار يُمرِّر وجهة Bin محدَّدة — فمتطلّب التاسك *"Enter source/target warehouse/zone/location"* **غير قابل للتحقيق حاليًا على مستوى الـBin**.

---

## 4. تدقيق متطلبات التاسك IC-1912 — بندًا ببند

| # | متطلّب التاسك | الحالة | الدليل |
|---|---|---|---|
| 1 | Enter source/target warehouse/**zone/location** and **reason** | ❌ | لا يوجد أي حقل `Reason`/`Remarks` على `InventoryTransfer`؛ ولا `BinLocationId` على السطر — الوجهة على مستوى Bin مستحيلة |
| 2 | Select/Scan items and qty from source | ⚠️ | مدعوم عبر الـDTO، لكن **لا يوجد Endpoint للبحث بالباركود** (نفس فجوة SRS 6) |
| 3 | availability at source checked **in real time** | ⚠️ | الفحص يحدث **فقط عند الحفظ** داخل `SaveInventoryMovement`/`HandleBinManagedLinesAsync` — **لا يوجد Endpoint استعلام مسبق** للتحقق الفوري أثناء الإدخال |
| 4 | insufficient-stock error blocks | ✅ | رسالتان مختلفتان: للعادي `"The appropriate quantity of this product is not available in this warehouse."`؛ للـBin `"Insufficient stock across bins under warehouse (Code: X)..."` |
| 5 | Inventory deducted at source & added at target | ✅ | GI ينقص `OnHand`، GR يزيده — عبر `SaveInventoryMovement` |
| 6 | **location history updated** | ⚠️ | سجل `InventoryMovement` يُنشَأ فعليًا (حركتان: Outbound + Inbound) **لكن:**<br>• `DocumentType` **لا يُعبَّأ إطلاقًا** في `SaveInventoryMovement` → يبقى `0` (قيمة غير صالحة في الـenum)<br>• `DocumentId` يُشير لمستند GI/GR **وليس لـ`InventoryTransfer`** → **لا يمكن الرجوع من الحركة إلى مستند النقل** |
| 7 | **Require approval for cross-warehouse transfers** | ❌ | لا يوجد `Status` ولا `ApprovalStatus` على `InventoryTransfer` إطلاقًا — البنية غير موجودة |
| 8 | View transfer history | ⚠️ | `GET /` و`GET /{id}` موجودان، لكن `DatabaseSelect` **لا يعمل `Include` للأسطر** → القائمة بلا تفاصيل |
| 9 | Stock movement report (source, target, item, qty, timestamp) filterable by warehouse/date | ❌ | **لا يوجد `InventoryMovementController` إطلاقًا** — الخدمة موجودة بلا أي Endpoint عام.<br>كما أن الحركة الواحدة تحمل مستودعًا **واحدًا** فقط (المصدر أو الهدف) — لعرض "source + target" في صف واحد يلزم ربطهما عبر مستند النقل، وهو الربط المكسور في البند 6 |

---

## 5. تدقيق الأداء — مخالفات نمط المؤلف (N+1 والاستعلامات المتكررة)

### 5.1 `InventoryTransferService.DatabaseGetById` — الأسوأ على الإطلاق
```csharp
foreach (var line in dto.InventoryTransferLines)
{
    line.FromWarehouse = await WarehouseService.Get(...);        // N
    line.ToWarehouse   = await WarehouseService.Get(...);        // N
    var product = await ProductService.GetByIdBasicInfo(...);    // N
    foreach (var sn in line.InventoryTransferLineSerialNumbers)
        sn.SerialNumber = await SerialNumberService.Get(...);    // N × M  ⚠️
}
```
**لنقل بـ20 سطرًا × 10 سيريالات = 60 + 200 = 260 استعلامًا منفصلًا.**
**النمط الصحيح الموجود بالفعل في المشروع:** `WarehouseService.GetWarehouseBinManagementInfoAsync(List<Guid>)` → استعلام واحد يُعيد `Dictionary`. يجب تطبيق نفس النمط للمستودعات والمنتجات والسيريالات (3 استعلامات مجمّعة بدل 260).

### 5.2 `SaveInventoryMovement` — استعلامان + حفظ منفصل لكل سطر
```csharp
foreach (var line in dto.InventoryTransactionLines)
{
    var productAvilability = await ProductService.GetProductAvailability(...);   // N
    ...
    await InventoryMovementService.Add(inventoryMovementDto);                    // N × (Add + SaveChangesAsync!)
}
```
`InventoryMovementService.DatabaseAdd` ينفّذ `SaveChangesAsync()` **لكل سجل على حدة** → N رحلات ذهاب وإياب لقاعدة البيانات.

### 5.3 `UpdateInventoryTransactionLine` — تعبئة القاموس تسلسليًا
```csharp
foreach (var productId in distinctProductIds)
    productsById[productId] = await ProductService.GetByIdBasicInfo(productId);   // N
```
القاموس صحيح كفكرة، لكن تعبئته تسلسلية — يلزم `GetByIdsAsync(List<Guid>)` مجمّعة.

### 5.4 `CheckSerialNumbers` / `CheckBatchNumbers` — استعلام لكل عنصر
`SerialNumberService.Get(snId)` ثم `Update(snId, ...)` لكل سيريال → **2 استعلام × عدد السيريالات**، ومثله للدفعات.

### 5.5 ازدواج العمل الكامل في النقل
`InventoryTransfer` يستدعي `GoodsIssuedService.Add` ثم `GoodsReceiptService.Add` — وكلٌّ منهما يُعيد تشغيل **كامل** `UpdateInventoryTransactionLine` (بما فيه جلب المنتجات وشجرة الـBins من جديد). **جلب `productsById` وشجرة الـBins يتكرر مرتين لكل نقل.**

---

## 6. ثغرات صحة ومتانة إضافية

| # | الثغرة | الموقع | الأثر |
|---|---|---|---|
| 1 | **5 كتل `catch (Exception) { }` فارغة** | `DatabaseGetById` | تُخفي أخطاء حقيقية (مستودع محذوف، منتج غير موجود) وتُعيد `null` صامتًا |
| 2 | **التحقق من وجود المستودعات مُعطَّل (Commented Out)** | `IsValidRecord:215-239` | يمكن إنشاء نقل بمستودع غير موجود |
| 3 | **لا تحقق من `From ≠ To`** | `IsValidRecord` | نقل من المستودع لنفسه مسموح (بلا معنى، وقد يُفسد المخزون) |
| 4 | **لا تحقق من `Quantity > 0`** | `IsValidRecord` | كمية صفر أو سالبة غير مرفوضة صراحة |
| 5 | **`Update`/`Patch`/`Delete` → `NotImplementedException`** | `InventoryTransferService` | لا يمكن تعديل أو إلغاء أي نقل بعد إنشائه |
| 6 | **لا `Status`** | `InventoryTransfer` | لا مسودّة، لا موافقة، لا إلغاء |
| 7 | **GI/GR لا يُربطان بالنقل** | `DatabaseAdd:118-124` | `BaseDocumentId = dto.BaseDocumentId` (مستند النقل الأصلي)، **وليس `entity.Id`** → التتبّع العكسي مكسور |

---

## 7. سجل الفجوات مرتّبًا (خطة التنفيذ المقترحة، بلا تطبيق)

| # | البند | الحجم | المخاطرة | الأولوية |
|---|---|---|---|---|
| 1 | **`InventoryTransferLineBatchNumber`** + تمرير `BatchNumbers` في `DatabaseAdd` (نسخ نمط `GoodsIssuesLineBatchNumbers` حرفيًا) | متوسط | منخفضة (إضافة بحتة) | 🔴 **حرجة** — تفتح نصف أنواع المواد |
| 2 | **`BinLocationId` على `InventoryTransferLine`** (مصدر وهدف) + احترامه في `HandleBinManagedLinesAsync` بدل التخصيص التلقائي عند تعبئته | متوسط | **منخفضة** إن بقي التلقائي هو الافتراضي عند الفراغ | 🔴 **حرجة** — تُصلح Bin→Bin |
| 3 | ربط GI/GR بمستند النقل (`BaseDocumentId = entity.Id`, `BaseDocumentType = InventoryTransfer`) + تعبئة `DocumentType` في `SaveInventoryMovement` | صغير | منخفضة | 🔴 عالية |
| 4 | `Reason` + `Status` + `ApprovalStatus` على `InventoryTransfer` (إعادة استخدام `ApprovalConfiguration`/`ApprovalHistory` القائمَين) | متوسط | منخفضة | 🟠 متوسطة |
| 5 | `InventoryMovementController` + فلاتر (Warehouse/Date/Product) عبر `QueryPayload` القائم | صغير | لا شيء | 🟠 متوسطة |
| 6 | Endpoint فحص توفّر مسبق (Real-time availability) | صغير | لا شيء | 🟠 متوسطة |
| 7 | تحسين `DatabaseGetById` بالاستعلامات المجمّعة + `Dictionary` (260 → 3 استعلامات) | متوسط | منخفضة | 🟠 متوسطة |
| 8 | تجميع `SaveInventoryMovement` (استعلام واحد للتوفّر + `AddRange` واحد للحركات) | متوسط | متوسطة (تمسّ دالة مشتركة مع GR/GI) | 🟡 لاحقة |
| 9 | تفعيل تحققات `IsValidRecord` المُعطَّلة + `From ≠ To` + `Quantity > 0` + إزالة `catch` الفارغة | صغير | منخفضة | 🟡 لاحقة |

---

## 8. الخلاصة النهائية

**ما ورثه Stock Transfer من عملنا في GR/GI ويعمل فعلًا:**
✅ تخصيص Bins التلقائي • فحص سعة الـBin • فحص نقص المخزون (عادي وBin) • نقل السيريالات مع تغيير الموقع والحالة • سجل حركة `InventoryMovement` لكل طرف • تحويل وحدات القياس • رفض SalesKit/VirtualItem.

**ما لم يرثه ويجب بناؤه:**
❌ الدفعات بالكامل • احترام الـBin المحدَّد يدويًا • الموافقات • السبب (Reason) • الحالة • ربط التتبّع • تقرير الحركة كـAPI • الفحص الفوري المسبق.

**التوصية:** البندان 1 و2 وحدهما يرفعان التغطية من **50% من أنواع المواد** إلى **100%**، ومن **دعم جزئي للـBins** إلى **دعم كامل ودقيق** — وكلاهما إضافات بنيوية تتبع نمط المؤلف حرفيًا (نسخ نمط `GoodsIssuesLineBatchNumbers` و`BinLocationId` الموجودَين أصلًا) بلا مساس بأي منطق قائم.
