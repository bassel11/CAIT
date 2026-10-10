# IC-2132 – الفلترة والترتيب حسب حالة المخزون في `shopping-products`

> **الحالة:** **مُطبَّق في 2026-10-11** (النسخة الرابعة). `ProductService.cs`: من `4081805d` إلى `ff5cca17`. `ProductDto.cs` لم يُمسّ. النسخة الاحتياطية مع `restore.ps1` و`applied.diff`: `_backups\IC-2132-TransactionFiltersAndSort-BEFORE-20261011-003326`.
> **الإثبات:** 76 فحصاً من 76 نجحت على **النموذج الحقيقي**. قاعدة LocalDB مؤقتة أُنشئت من `ITM.Core.Organization.Database 26.1008.2704`، والاستعلام يمرّ في `QueryHelper.GenerateQuery` الحقيقي (`ITM.Core.Organization 26.1008.2718`) مع `ProductDto` الحقيقي (`ITM.Core.Products 26.1009.2721`). المقترح نفسه **لا يستدعي أي دالة من دوال المؤلف**. التقرير الكامل في الملحق [IC-2132-Stock-Status-Test-Report.md](IC-2132-Stock-Status-Test-Report.md).

---

## 0. الخلاصة

1. **الملفات أصلية بايتاً ببايت:** `ProductService.cs` = `4081805d…` و`ProductDto.cs` = `19326988…`.
2. ⚠️ **مسودة الترتيب في الأسطر 228–236 ما زالت في الملف المسترجَع.** هي ترتّب بالـ GUID وتُلغي كل ترتيب ترسله الواجهة. **لا تنشر الملف كما هو** (القسم 1.2).
3. **التصميم النهائي** (القسم 7): دالة خاصة واحدة `ApplyTransactionFiltersAndSort` يستدعيها سطر واحد. **تشكّل الاستعلام بـ LINQ فقط:**
   - `Where` و`OrderBy` على إسقاطين مجهولين.
   - المجموع عبر الملاحة كما في `GetAvailability`.
   - ثلاثي `? :` ثم `Sort = null` كما في ترتيب السعر.
   - لا `FilterBuilder` ولا `PropertyCache` ولا أي دالة للمؤلف، ولا `from … join`، ولا صنف أو DTO أو حقل جديد.
   - **`ProductDto` لا يُمسّ.**
4. **الحقلان الموجودان فقط:**
   - `IsAvailable`: فلتر بـ `Equals`/`NotEquals`/`In`/`NotIn`، وترتيب بالتوفّر (الكمية) كما في مسودة المؤلف.
   - `StockStatusLabel`: فلتر بـ `Equals`/`NotEquals`/`In`/`NotIn`/`Contains`/`StartsWith`/`EndsWith`.
   - تحت ترتيب التوفّر نسخة معلّقة: الحالة المخصّصة ثم التوفّر.
5. **`decimal` وليس `long`:** `(long?)` عند المؤلف يقطع كسر `OnHand` وحده، والنتيجة `decimal`. نستعمل التعبير نفسه حرفياً، فالنوع والقيم مطابقة (القسم 3.1.1).
6. **كود المؤلف لا يُمسّ:**
   - فلتر قائمة الأسعار و`GetStockStatus` و`GenerateQuery` والحلقة كما هي.
   - ترتيب السعر (IC-2064) نُقل حرفياً.
   - كل طلب لا يذكر الحقلين يعطي **نص SQL مطابقاً حرفياً** (14 من 14).
7. **لا استعلام إضافي إلا عند الحاجة.** على 20,000 منتج:
   - `IsAvailable`: من 249 إلى 915 ms.
   - فلترة النص: من 2 إلى 4 ثوانٍ، بسبب تقنية المسودة (القسم 7.6).
   - على 2,000 منتج: كل شيء تحت 0.5 ثانية.
8. **`CreateDate` معلّق** في الموضعين (القسم 9).
9. **التطبيق ينتظر "طبق"** (القسم 12).

---

## 1. ما الذي استرجعته بالضبط

### 1.1 الملفات

| الملف | قبل الاسترجاع (نسخة الحقل النصي) | بعد الاسترجاع (الأصل) |
|---|---|---|
| `Services/ProductService.cs` | `5f30ac55…` | **`4081805d…`** = نسخة BEFORE من أول نسخة احتياطية لـ IC-2132، بايتاً ببايت |
| `DTOs/ProductDto.cs` | `db7f3199…` | **`19326988…`** = الأصل، بلا حقل وبلا `using` |
| `Helper/ExpressionExtensions.cs` | `8c68e146…` | محذوف (أضفناه نحن) |
| `Enums/StockStatusType.cs` | كان محذوفاً أصلاً | غير موجود. في المجلد `OperationType.cs` فقط، كما كان |

نسخة الحالة السابقة (الحقل النصي) محفوظة كاملة وقابلة للإرجاع:
`_backups\IC-2132-RestoreOriginal-BEFORE-20261010-154319\restore.ps1`

### 1.2 ⚠️ مسودة الترتيب في الأسطر 228–236 من الملف المُسترجَع

```csharp
query = query.OrderBy(x => dbContext.CustomStockStatuses
 .Where(c =>
 c.QuantityFrom <= x.ProductAvailabilities.Sum(a => (decimal?)(a.OnHand - a.Committed)) &&
 c.QuantityTo >= x.ProductAvailabilities.Sum(a => (decimal?)(a.OnHand - a.Committed)))
.FirstOrDefault())
.ThenBy(x => x.ProductAvailabilities.Sum(a => (decimal?)(a.OnHand - a.Committed)));

if(queryPayload.Sort != null)
    queryPayload.Sort = null;
```

هذه مسودة المؤلف غير المنتهية، وكانت في الملف قبل أول تعديل لي، لذلك عادت مع الاسترجاع الحرفي. ما تفعله فعلياً، **مُثبَت على النموذج الحقيقي** (القسم 2 من التقرير):

| السلوك | الدليل |
|---|---|
| يعمل **دائماً**، حتى بلا أي طلب ترتيب، فيغيّر ترتيب القائمة الافتراضي (`CreateDate desc`) | الطلب الافتراضي: ترتيب المؤلف `Zero Stock Backorder, Oversold Item, …` وترتيب الملف المُسترجَع `Committed Exceeds Item, Oversold Item, …` |
| `OrderBy(... .FirstOrDefault())` على كيان **يرتّب بمعرّف الحالة (GUID)**، فلا معنى للترتيب | SQL: `ORDER BY (SELECT TOP(1) [c].[Id] FROM [CustomStockStatuses] …)` |
| يُلغي **كل** ترتيب ترسله الواجهة (`Sort = null`)، فلا يعود `Sort.SortBy=Name` يعمل | `Sort Name asc`: المؤلف `Alex Vando Shirt, …` والملف المُسترجَع ترتيب GUID |
| يُلغي ترتيب السعر (IC-2064)، لأن `OrderBy` الثاني يمحو الأول | `Sort ProductPrices.UnitPrice asc`: النتيجتان مختلفتان |
| صيغته تختلف عن صيغة المؤلف: `OnHand - Committed` بلا `(long?)`، وبلا مستودع | SQL: `SUM([p0].[OnHand] - [p0].[Committed])` بلا `CAST … bigint` وبلا `WarehouseId` |
| يحذف الترتيب من الصدى المرسل للواجهة | echo المؤلف `"Sort":{"SortBy":"CreateDate",…}` والملف المُسترجَع بلا `Sort` |

**ما نأخذه من المسودة وما نصحّحه** (هي مسودة المؤلف غير المنتهية):

| في المسودة | في المقترح |
|---|---|
| تعمل دائماً | تعمل فقط حين يطلب الطلب `Sort.SortBy=IsAvailable` |
| `OrderBy(الحالة)` بالكيان، أي بالـ GUID | النسخة المعلّقة ترتّب **بنص** الحالة ثم بالكمية |
| `ThenBy(Sum(OnHand - Committed))` | الترتيب الفعّال بالكمية، **بصيغة `GetAvailability`** (`(long?)OnHand - Committed`، والمستودع، و`?? 0`) |
| `Sort = null` دائماً | `Sort = null` فقط بعد ترتيبنا، كما في ترتيب السعر |

**المقترح في القسم 7 يحذف المسودة ويحلّ محلها.** وإن أردتَ الآن الملف الأصلي **بدونها**، قل ذلك وأحذفها وحدها مع نسخة احتياطية.

---

## 2. شرح الدالة الأصلية سطراً بسطر (`ProductService.cs`، `4081805d`)

### 2.1 نقطتا الدخول

| السطر | الكود | المعنى |
|---|---|---|
| 140–143 | `ShoppingProducts(...)` ← `GetProductsForTransaction(..., p => p.SalesItem == true)` | قائمة البيع (`GET api/Products/shopping-products`) |
| 145–148 | `PurchasingProduct(...)` ← `..., p => p.PurchasingItem == true)` | قائمة الشراء (`GET api/Products/purchasing-products`). **كل ما نضيفه يعمل في القائمتين**، لأنه داخل الدالة المشتركة |

المتحكّم يربط الطلب بـ `[FromQuery] QueryPayload queryPayload`، فالواجهة ترسل المعاملات في الرابط: `Sort.SortBy=…&Filter.Conditions[0].Field=…`.

### 2.2 الدالة `GetProductsForTransaction` (الأسطر 150–271)

| السطر | ما يحدث | ملاحظات دقيقة |
|---|---|---|
| 152 | `QueryHelper.GetConfigurationQueryPayload(queryPayload)` | من الحزمة: إن لم يُرسل ترتيب يصبح `Sort = CreateDate desc`، وإن كان `PageSize > 200` أو `< 0` يصبح 50، وإن كان `PageNumber < 1` يصبح 1. **لذلك لا يكون `Sort` فارغاً أبداً بعد هذا السطر** |
| 153–156 | إن كان المستخدم تابعاً لشركة يُفرض `companyId` الخاص به | |
| 158–162 | `parsedWarehouseId = new Guid(warehouseId)` | قيمة غير صالحة تعطي `FormatException` (500). هذا سلوك المؤلف |
| 164–176 | السعر: قائمة أسعار الشركة (أو `Guid.Empty`)، وإلا قائمة الأسعار الافتراضية | يُنتج `priceListId` الذي يُستعمل في فلتر السعر وترتيبه |
| 178–189 | **فلتر مخصّص للمؤلف:** إن وُجد شرط على `ProductPrices.UnitPrice` في المستوى الأعلى يُضاف شرط `ProductPrices.PriceListId Equals priceListId` | `FilterBuilder` يجمع الشروط على مجموعة `ProductPrices` في `Any(...)` واحد، فيُفحص السعر **في صف قائمة الأسعار نفسها**. ملاحظتان عن كود المؤلف لم أغيّرهما: (أ) شرطان على `UnitPrice` (مدى: `GreaterThan` و`LessThan`) يضعهما `NormalizeFilterGroup` في مجموعة فرعية، فيصبحان `Any` منفصلاً عن شرط القائمة. (ب) مع `Filter.Logic=OR` يُضمّ شرط القائمة بـ OR |
| 191–195 | الاستعلام الأساسي: `AsNoTracking`، و`Include(TaxClass)` و`Include(InventoryUnitOfMeasurement)`، ثم `productFilter`، ثم المنتج نشط وغير `NotVisible` ومن التصنيف المطلوب | |
| 197–212 | الكتالوج المطلوب، أو كتالوجات الشركة إن كانت لها كتالوجات | |
| 215–218 | فلتر قائمة الأسعار مُعلَّق | يبقى معلّقاً، لأنه يُخفي المنتجات غير المسعّرة |
| 220–226 | **ترتيب مخصّص أضفناه نحن في IC-2064 (2026-09-29) وقبلتموه.** ليس من كود المؤلف الأصلي، فنسخة ما قبل IC-2064 لا تحويه. يرتّب `ProductPrices.UnitPrice` بسعر قائمة الطلب، ثم `Sort = null` | بدون هذا التدخّل يرتّب `SortBuilder` الخاص بالحزمة المسار المتداخل `ProductPrices.UnitPrice` بأصغر سعر **في كل القوائم**. و`Sort = null` يمنع `GenerateQuery` من محو هذا الترتيب |
| 228–236 | مسودة ترتيب غير منتهية (الترتيب بالحالة المخصّصة ثم بمجموع التوفّر) | القسم 1.2. استُوحي منها ترتيب المقترح |
| 238 | `QueryHelper.GenerateQuery(...)` | القسم 2.3 |
| 240 | `ToListAsync()` | |
| 242 | AutoMapper: `Product` إلى `ProductDto` | الحقول المحسوبة (`IsAvailable`، `StockStatusLabel`، …) لا مصدر لها في `Product`، فتُملأ في الحلقة |
| 243–263 | **حلقة لكل منتج:** `GetFinalPrice`، ثم `GetStockStatus(p, parsedWarehouseId)` (التوفّر والنص واللون)، ثم `IsWishlisted`، ثم `MainImageUrl` | كل منتج يكلّف عدة رحلات إلى القاعدة (القسم 10) |
| 265–270 | الاستجابة | `TotalCount = entities.Count` هو عدد عناصر **الصفحة**. العدد الكلي الحقيقي في `QueryPayload.TotalCount` الذي يملؤه `GenerateQuery` |

### 2.3 ما يفعله `GenerateQuery` من الداخل (فككتُ الحزمة وقرأتها)

ترتيب التنفيذ الحرفي:

1. **الفلتر:**
   - `ExtractMetafieldGroup` تُخرج شروط الحقول الوصفية (`IsMetaField`).
   - `NormalizeFilterGroup` تجمع الشروط حسب `Field`:
     - شرطان على الحقل نفسه بالعملية نفسها، أو أحدهما `IsNull`/`NotNull`: يُدمجان بـ **OR**.
     - عمليات مختلفة على الحقل نفسه: **AND** بين العمليات، و**OR** داخل كل عملية.
   - ثم `FilterBuilder.BuildPredicate<Product, ProductDto>`، وهي تشترط لكل حقل:
     - أن تحمل خاصية `ProductDto` الوسم `[AllowFilter]`، وإلا: `Filtering by 'X' is not allowed.`
     - أن يكون في `Product` عمود بالاسم نفسه، وإلا: `Filtering by 'X' is not supported.`
   - **لذلك يرفض المؤلف اليوم `IsAvailable` و`StockStatusLabel` بخطأ 400.**
2. `OnlyTotalNumber`: عدّ فقط.
3. **الترتيب:**
   - حقل وصفي، أو `SortBuilder.ApplySort`.
   - مفتاح واحد فقط (`OrderBy`)، بلا مفتاح ثانٍ أبداً.
   - يشترط `[AllowSorting]` وعموداً مطابقاً، وإلا: `Sorting by 'X' is not supported.`
4. **البحث** (`Search.Value`):
   - تُقسَّم الكلمات بالمسافة.
   - كل كلمة تُطبَّق بـ `Contains` على كل حقل عليه `[AllowSearch]`، والنتائج كلها تُضمّ بـ **OR**.
5. `CountAsync`، والنتيجة تذهب إلى `queryPayload.TotalCount`.
6. `Skip`/`Take` للصفحة.
7. التصدير (Excel/PDF): يرجع الاستعلام هنا.
8. `Expand` يضيف `Include`.
9. `Fields` يطبّق الإسقاط (projection).

**النتيجة:** أي حقل ليس عموداً لا بد أن يُعالَج **قبل** `GenerateQuery`، ثم يُحذف من الطلب. هذا بالضبط ما يفعله المؤلف لترتيب السعر، وما تفعله الحزمة نفسها للحقول الوصفية: تُخرجها، وتطبّقها بـ `EXISTS`، ثم تعيدها إلى الطلب.

### 2.4 الدوال التي تعرض حالة المخزون (لم ولن تُمسّ)

```csharp
// السطر 2244
public async Task<(bool, int)> GetAvailability(Guid productId, Guid? warehouseId = null)
{
    if (warehouseId == Guid.Empty) warehouseId = null;
    var availableQuantity = await InitialDbSet().AsNoTracking().Where(p => p.Id == productId)
        .Select(p => p.ProductAvailabilities
            .Where(pa => warehouseId == null || pa.WarehouseId == warehouseId)
            .Sum(pa => (long?)pa.OnHand - pa.Committed) ?? 0)
        .FirstOrDefaultAsync();
    return (availableQuantity > 0, (int)availableQuantity);
}

// السطر 2855
private async Task GetStockStatus(ProductDto dto, Guid? warehouseId = null, int? quantity = null)
{
    var definitions = await _productServiceContext.CustomStockStatusService.Select(new QueryPayload()); // أحدث 50 حالة
    var res = await GetAvailability(dto.Id, warehouseId);
    dto.IsAvailable = res.Item1;            // الكمية > 0
    var currentQuantity = res.Item2;        // الكمية مقطوعة إلى int
    // أول حالة مخصّصة يحوي مداها الكمية، وإلا GetDefaultStockLabel؛ اللون من الحالة المخصّصة فقط
    ...
}

// السطر 2887
private string GetDefaultStockLabel(ProductDto dto)  // IsAvailable ← "Available"، وإلا AllowBackorder ← "Back order"، وإلا "Out of Stock"
```

---

## 3. العلاقة الدقيقة بين التوفّر (`Sum(OnHand − Committed)`) و`CustomStockStatus`

### 3.1 الكمية المتوفرة (`GetAvailability`)

$$\text{available} = \sum_{\text{صفوف المنتج في المستودع المطلوب، أو كل المستودعات}} \big(\text{truncate}(\text{OnHand}) - \text{Committed}\big)\quad;\quad \text{بلا صفوف} = 0$$

- `(long?)pa.OnHand - pa.Committed`: التحويل `(long?)` يطبَّق على **`OnHand` وحده**، أي يُقطع كسره، بينما يبقى `Committed` عشرياً. في SQL: `CAST(CAST(OnHand AS bigint) AS DECIMAL(18,2)) - Committed`.
- `warehouseId` يأتي من معامل الرابط `warehouseId=`. بدونه تُجمع كل المستودعات، و`Guid.Empty` يعامَل كأنه بلا مستودع.

#### 3.1.1 النوع: `decimal` وليس `long` (مُثبَت بفحص المترجم)

التحويل `(long?)` في `(long?)pa.OnHand - pa.Committed` يطبَّق على **`OnHand` وحده**، لأن التحويل أعلى أولوية من الطرح. وطرح `decimal` من `long?` يحوّل `long?` تلقائياً إلى `decimal?`. فالنتيجة `decimal`:

| التعبير | النوع (طبعه المترجم من شجرة التعبير) |
|---|---|
| `(long?)pa.OnHand` | `Nullable<Int64>` |
| `(long?)pa.OnHand - pa.Committed` | **`Nullable<Decimal>`** |
| `.Sum((long?)pa.OnHand - pa.Committed)` | `Nullable<Decimal>` |
| `… ?? 0` (‏`availableQuantity` عند المؤلف) | **`Decimal`** |

شجرة التعبير نفسها: `Convert(Convert(OnHand, Int64), Nullable<Decimal>) - Convert(Committed, Nullable<Decimal>)`. وفي SQL: `CAST(CAST(OnHand AS bigint) AS DECIMAL(18,2)) - Committed`.

**`long` يقطع كسر `OnHand` فقط، ولا يجعل النتيجة عدداً صحيحاً.** ولو استعملنا `long` للنتيجة لاختلفنا عن المؤلف:

| OnHand | Committed | المؤلف (`decimal`) | `IsAvailable` | `(int)` للنص | لو كانت `long` |
|---|---|---|---|---|---|
| 1 | 0.5 | **0.5** | ✔ متوفر | 0 | 0 فيصبح **غير متوفر** ✘ مخالف للمعروض |
| 4.5 | 0 | 4 | ✔ | 4 | 4 |
| 2 | 0.25 | 1.75 | ✔ | 1 | 1 |
| 5 | 8 | −3 | ✘ | −3 | −3 |

لذلك يستعمل المقترح التعبير **حرفياً** كما في `GetAvailability`، فالكمية `decimal`:
- `IsAvailable = Quantity > 0` يقارن `decimal` كما يفعل `availableQuantity > 0`.
- النص يقارن `(int)Quantity` كما يفعل `currentQuantity = (int)availableQuantity`.

### 3.2 من الكمية إلى ما يُعرض

| الحقل في الاستجابة | القاعدة |
|---|---|
| `isAvailable` | `available > 0` (المقارنة على القيمة العشرية) |
| `stockStatusLabel` | أول حالة مخصّصة يتحقق فيها `QuantityFrom ≤ (int)available ≤ QuantityTo` (الحدّان داخلان، والقطع نحو الصفر)، بين **أحدث 50 حالة** مرتّبة `CreateDate desc`. إن لم توجد: `Available` إذا كان `isAvailable`، وإلا `Back order` إذا كان `AllowBackorder`، وإلا `Out of Stock` |
| `stockStatusColor` | لون الحالة المخصّصة، وإلا `null` |

- `IsValidRecord` في `CustomStockStatusService` يمنع **تداخل المديات** و`From > To`، ويفرض **تفرّد النص**.
- الأعمدة `QuantityFrom`/`QuantityTo` من نوع `int`، بينما الـ DTO من نوع `decimal`.

### 3.3 أمثلة من بيانات الاختبار

المديات المفترضة هي نصوصك، لكن المديات نفسها تقدير مني: `Available brian 1` = 1..5، و`JLoka-01` = 10..99، و`JLoka-02` = 100..500.

| المنتج | صفوف المخزون | available | isAvailable | stockStatusLabel | لماذا |
|---|---|---|---|---|---|
| Jafar-Product-03 | W1: 1000 − 0 | 1000 | ✔ | Available | 1000 خارج كل المديات (فوق 500)، فيُستعمل النص الافتراضي |
| Loka-01 | W1: 200 − 0 | 200 | ✔ | JLoka-02 | 200 داخل 100..500 |
| matt test child 2 | W1: 7 − 0 | 7 | ✔ | Available | 7 في **الفجوة** 6..9 |
| OnHand 4.5 Item | W1: 4.5 − 0 | **4** | ✔ | Available brian 1 | `OnHand` يُقطع: 4.5 تصبح 4 |
| Half Unit Item | W1: 1 − 0.5 | **0.5** | ✔ | Available | 0.5 > 0 فهو متوفر، و`(int)0.5 = 0` لا يقع في أي مدى، فالنص الافتراضي |
| Committed Exceeds Item | W1: 5 − 8 | −3 | ✘ | Back order | سالب، و`AllowBackorder` |
| Oversold Item | W1: 2 − 5 | −3 | ✘ | Out of Stock | سالب، وبلا `AllowBackorder` |
| Out Of Stock No Rows | لا صفوف | 0 | ✘ | Out of Stock | |
| Multi Warehouse Item | W1: 3، W2: 60 | 63 (الكل) / 3 (W1) | ✔ | JLoka-01 (الكل) / Available brian 1 (W1) | **المستودع يغيّر النص** |

---

## 4. لماذا ظهر `Available ← JLoka-02 ← JLoka-01… ← Available ← Available brian 1…`

الخرج الذي أرسلته في 2026-10-09 (`Sort.SortBy=StockStatus&Sort.Ascending=false`) جاء بهذا الترتيب: Available (Jafar)، ثم JLoka-02 (Loka-01)، ثم JLoka-01 ×4، ثم Available (matt)، ثم Available brian 1 ×12.

**السبب رياضي وليس خطأ:** ذلك الترتيب كان بالمجموعة ثم **بالكمية تنازلياً**. الحالات المخصّصة **مديات على محور الكمية**، والنص الافتراضي `Available` يملأ **الفجوات** بينها:

```
الكمية:   1      5 6    9 10          99 100        500 501 …
          |brian |  Avail |   JLoka-01   |  JLoka-02   | Available …
                    (فجوة)                               (فوق آخر مدى)
```

حين تُرتَّب بالكمية تنازلياً تعبر هذا المحور من اليمين إلى اليسار، فتمرّ بـ `Available` أكثر من مرة. أعدتُ إنتاج خرجك حرفياً على النموذج الحقيقي (الحالة S1 في التقرير):

| # | المنتج | الكمية | النص |
|---|---|---|---|
| 1 | Jafar-Product-03 | 1000 | **Available** |
| 2 | Loka-01 | 200 | JLoka-02 |
| 3–7 | Test-MetaField, Multi Warehouse, Plug, Alex Red, Alex | 80…12 | JLoka-01 |
| 8 | matt test child 2 | 7 | **Available** |
| 9–13 | iphone caver, OnHand 4.5, Nitrile, Paintbrush, Scrub | 5…1 | Available brian 1 |
| 14 | Half Unit Item | 0.5 | **Available** |

**لا يمكن الحصول على الأمرين معاً**، أي ترتيب بالكمية وكل نص في كتلة واحدة، لأن النص `Available` نفسه موزّع على مجالات غير متصلة. لذلك في المقترح النهائي:

- **`Sort.SortBy=IsAvailable` (فعّال): التوفّر**، أي الكمية، كما في `ThenBy` مسودة المؤلف. هذا الجدول نفسه (S1). النصوص قد تتكرّر، وهذا متوقع.
- **تحته نسخة معلّقة: الحالة ثم التوفّر.** كل نص كتلة واحدة، ثم الكمية داخلها (القسم 8.4):

  | تصاعدي | Available (0.5, 7, 1000) ← Available brian 1 (1…5) ← Back order ← JLoka-01 (12…80) ← JLoka-02 ← Out of Stock |
  |---|---|

---

## 5. لماذا اعترض المؤلف على الـ Enum (تحليل)

لا أعرف نص اعتراضه، لكن هذه أسباب تقنية تكفي وحدها لرفضه، ومنها ما ثبت عندنا:

1. **حالة المخزون في هذا النظام بيانات وليست قائمة ثابتة.** النصوص والمديات والألوان يُدخلها المستخدم في جدول `CustomStockStatuses`. enum من ثلاث قيم (`Available`/`BackOrder`/`OutOfStock`) لا يمثّل ما يُعرض فعلاً.
2. **تكرار وتناقض:** الاستجابة فيها `stockStatusLabel` أصلاً. حقل ثانٍ يصف الشيء نفسه قد يعارضه: النص `JLoka-01` والحقل `Available`، أو النص `Sold out` على مدى يحوي 0 والحقل `Available` كما في مثال Half Unit.
3. **سطح عام لحزمة مشتركة:** `ProductDto` منشور في حزمة يستهلكها Commerce (`26.1009.2721`) وPOD (`26.1007.2690`). نوع جديد وحقل جديد يعنيان توافقاً ثنائياً ورفع إصدار وأثراً في كل الاستجابات.
4. **آثار جانبية رأيناها:**
   - حقل `stockStatus: null` في كل استجابات `ProductDto` خارج القائمتين.
   - عمود فارغ في التصدير الافتراضي.
   - سطر زائد داخل حلقة المؤلف.

**النتيجة:** حذف الـ enum صحيح. والأفضل **ألا نضيف حقلاً أصلاً**، وهذا هو القسم التالي.

---

## 6. هل نحتاج حقلاً جديداً في `ProductDto`؟ تحليل كل المسارات

| السؤال | الجواب والدليل |
|---|---|
| بأي اسم ترسل الواجهة حقل الفلترة أو الترتيب؟ | الحزمة تحلّ الاسم على **خصائص `ProductDto`**، دون حساسية لحالة الأحرف. فنمط الواجهة هو: الاسم = اسم الحقل الذي تستلمه |
| هل في الاستجابة ما يمثّل حالة المخزون؟ | نعم: `isAvailable`، و`stockStatusLabel`، و`stockStatusColor`، و`allowBackorder`. والأخير يُفلتر ويُرتَّب أصلاً (`[AllowFilter][AllowSorting]`) |
| أين تُملأ هذه الحقول؟ | `GetStockStatus` تُستدعى في 6 مسارات: قائمة البيع وقائمة الشراء، وتفاصيل المنتج، والعروض، والمتغيرات، والعلاقات، و`GetByIdBasicInfo` (أسطر السلة في Commerce). **القائمتان فقط تحتاجان الفلترة والترتيب**، وكلتاهما في `GetProductsForTransaction` |
| هل في المشاريع كود واجهة يقرأ الحقول من الـ DTO؟ | لا يوجد كود واجهة في `D:`. `DTOFIeldsHelper` في Portal يقرأ الوسوم لكن **لا أحد يستدعيه**. و`Sortable` في Layout إعداد يحفظه المسؤول، لا يُستنتج من الـ DTO |
| ماذا يكلّف حقل جديد؟ | يظهر في كل استجابات `ProductDto` في كل المسارات والمشاريع، ويضيف عموداً في التصدير، ويغيّر عقد حزمة منشورة، ويكرر النص ويتناقض معه |
| هل نضع `[AllowFilter]` على `IsAvailable`/`StockStatusLabel`؟ | **لا.** الدالة تشكّل الشروط بـ LINQ بنفسها ولا تستدعي `FilterBuilder`، فلا حاجة لأي وسم. **`ProductDto` لا يُمسّ إطلاقاً** |

**القرار:** **لا حقل جديد ولا DTO جديد ولا enum.** الواجهة ترسل:

- `IsAvailable`: فلتر (متوفر / غير متوفر) وترتيب بالتوفّر (الكمية).
- `StockStatusLabel`: فلتر نصي كامل على نص الحالة كما يُعرض.
- `AllowBackorder`: كما هو اليوم.

---

## 7. التصميم النهائي: دالة خاصة واحدة تشكّل الاستعلام بـ LINQ فقط

### 7.1 من أين جاء كل قرار

| المطلوب | القرار | الدليل |
|---|---|---|
| لا حقل ولا DTO ولا صنف جديد | إسقاطان مجهولان (`Select(p => new { … })`) داخل الدالة | المؤلف يستعمل `Select(x => new { … })` 22 مرة في المشاريع |
| **لا استدعاء لأي دالة من دوال المؤلف** (`FilterBuilder`، `PropertyCache`، `SortBuilder`…) | الشروط والترتيب مكتوبة بـ `Where`/`OrderBy` مباشرة على الاستعلام. لذلك **لا وسوم ولا تعديل على `ProductDto` إطلاقاً** | — |
| لا `from … join` | لا Query Syntax ولا `Join` ولا `GroupBy` في SQL | المؤلف لم يكتب Query Syntax ولا مرة (0) |
| تقنية المؤلف في التوفّر | `p.ProductAvailabilities.Where(…).Sum(pa => (long?)pa.OnHand - pa.Committed) ?? 0` حرفياً | `GetAvailability` (السطر 2244) و`CheckIsAvailable` ومسودته |
| `long` أم `decimal`؟ | **`decimal`، كما عند المؤلف** (القسم 3.1.1) | فحص النوع: `(long?)` يقطع كسر `OnHand` وحده، ثم تتحوّل القيمة إلى `decimal?` قبل الطرح |
| الترتيب حقل واحد | `if` لكل اسم ترتيب | `QueryPayload.Sort` كائن واحد وليس قائمة |
| الترتيب بالتوفّر كما في المسودة | `IsAvailable` يُرتَّب بالكمية | مسودة المؤلف: `.ThenBy(x => x.ProductAvailabilities.Sum(…))` |
| نسخة معلّقة بالحالة ثم التوفّر | تحت الترتيب مباشرة | المسودة: `.OrderBy(CustomStockStatuses…).ThenBy(Sum)`، مع الترتيب بالنص بدل الكيان (الكيان كان يرتّب بالـ GUID) |
| نقل ترتيب السعر | منقول حرفياً | أضفناه نحن في IC-2064. فلتر قائمة الأسعار (178–189) للمؤلف، وبقي مكانه |
| لا استعلام مرهق إلا عند الحاجة | الإسقاط والمجموع فقط إن ذكر الطلب الحقلين. نص الحالة لا يدخل SQL إلا إن فُلتر عليه | القسم 7.5 |
| لا شروط زائدة | لا `IsMetaField` ولا أي فحص لا يخصّنا | — |

### 7.2 ما يتغيّر: ملف واحد، `ProductService.cs`، في موضعين

**(أ) داخل `GetProductsForTransaction`:** الأسطر 220–236 (ترتيب السعر ومسودة الترتيب) يحلّ محلها سطر واحد، ويبقى كل ما عداها حرفياً:

```diff
-            if (queryPayload.Sort != null && queryPayload.Sort.SortBy.Equals("ProductPrices.UnitPrice", …))   // ← نُقل حرفياً إلى الدالة
-            { … }
-
-            query = query.OrderBy(x => dbContext.CustomStockStatuses …  (مسودة 228-236)                    // ← استُوحي منها الترتيب
-            if(queryPayload.Sort != null)
-                queryPayload.Sort = null;
+            // the filters and the sort GenerateQuery cannot build (the stock status, the price of the price list)
+            query = ApplyTransactionFiltersAndSort(query, queryPayload, priceListId, parsedWarehouseId);
 
             query = await QueryHelper<Product, ProductDto>.GenerateQuery(queryPayload, query, _mapper, dbContext);
```

**(ب) دالة خاصة جديدة بعد `GetProductsForTransaction`** (القسم 7.3).

- **لا `using` جديد:** `CustomInvalidDataException`، و`FilterCondition`/`ComparisonOperator`/`LogicalOperator`، و`ProductDto`، كلها في `using` الموجودة.
- **`ProductDto` لا يُمسّ.**

### 7.3 الدالة كاملة (هي النص المُختبَر حرفياً)

```csharp
        // The filters and the sort of the shopping and purchasing lists that are not columns of Product, so GenerateQuery cannot build them:
        // the stock status GetStockStatus shows (IsAvailable, StockStatusLabel) and the price of the request's price list.
        // The stock status is computed in the query (GetAvailability) only when the request filters or sorts on it.
        private IQueryable<Product> ApplyTransactionFiltersAndSort(IQueryable<Product> query, QueryPayload queryPayload, Guid priceListId, Guid? warehouseId)
        {
            var stockConditions = queryPayload.Filter?.Conditions
                .Where(x => x.Field.Equals(nameof(ProductDto.IsAvailable), StringComparison.OrdinalIgnoreCase) || x.Field.Equals(nameof(ProductDto.StockStatusLabel), StringComparison.OrdinalIgnoreCase))
                .ToList() ?? [];
            var sortByAvailability = queryPayload.Sort != null && queryPayload.Sort.SortBy.Equals(nameof(ProductDto.IsAvailable), StringComparison.OrdinalIgnoreCase);

            if (stockConditions.Count > 0 || sortByAvailability)
            {
                if (warehouseId == Guid.Empty)
                    warehouseId = null;

                // GetAvailability and GetStockStatus for each product, as the list shows them
                var products = query
                    .Select(p => new
                    {
                        Product = p,
                        Quantity = p.ProductAvailabilities
                            .Where(pa => warehouseId == null || pa.WarehouseId == warehouseId)
                            .Sum(pa => (long?)pa.OnHand - pa.Committed) ?? 0
                    })
                    .Select(x => new
                    {
                        x.Product,
                        x.Quantity,
                        IsAvailable = x.Quantity > 0,
                        StockStatusLabel = dbContext.CustomStockStatuses
                            .Where(c => c.QuantityFrom <= (int)x.Quantity && c.QuantityTo >= (int)x.Quantity)
                            //.OrderByDescending(c => c.CreateDate)
                            .Select(c => c.Label)
                            .FirstOrDefault() ?? (x.Quantity > 0 ? "Available" : x.Product.AllowBackorder ? "Back order" : "Out of Stock")
                    });

                // each condition narrows the list (AND); under OR it would have to be joined with the other conditions, which GenerateQuery builds
                if (stockConditions.Count > 0 && queryPayload.Filter!.Logic == LogicalOperator.OR && (queryPayload.Filter.Conditions.Count > 1 || queryPayload.Filter.Groups.Count > 0))
                    throw new CustomInvalidDataException($"Filtering by '{stockConditions[0].Field}' is supported only with AND logic.");

                foreach (var condition in stockConditions)
                {
                    if (condition.Field.Equals(nameof(ProductDto.IsAvailable), StringComparison.OrdinalIgnoreCase))
                    {
                        if (condition.Operator is not (ComparisonOperator.Equals or ComparisonOperator.NotEquals or ComparisonOperator.In or ComparisonOperator.NotIn))
                            throw new CustomInvalidDataException("IsAvailable can be filtered only with Equals, NotEquals, In or NotIn.");

                        var isAvailable = Values(condition).Select(x => bool.TryParse(x, out var value) ? value : throw new CustomInvalidDataException($"Invalid IsAvailable value: '{x}'. Allowed values are: true, false.")).Distinct().ToList();
                        var include = condition.Operator is ComparisonOperator.Equals or ComparisonOperator.In;
                        // true and false together: every product (Equals, In) or none (NotEquals, NotIn)
                        if (isAvailable.Count == 2)
                            products = include ? products : products.Where(x => false);
                        else
                            products = isAvailable[0] == include ? products.Where(x => x.IsAvailable) : products.Where(x => !x.IsAvailable);
                    }
                    else
                    {
                        if (condition.Operator is not (ComparisonOperator.Equals or ComparisonOperator.NotEquals or ComparisonOperator.In or ComparisonOperator.NotIn or ComparisonOperator.Contains or ComparisonOperator.StartsWith or ComparisonOperator.EndsWith))
                            throw new CustomInvalidDataException("StockStatusLabel can be filtered only with Equals, NotEquals, In, NotIn, Contains, StartsWith or EndsWith.");

                        var labels = Values(condition);
                        var text = labels[0];
                        products = condition.Operator switch
                        {
                            ComparisonOperator.Equals or ComparisonOperator.In => products.Where(x => labels.Contains(x.StockStatusLabel)),
                            ComparisonOperator.NotEquals or ComparisonOperator.NotIn => products.Where(x => !labels.Contains(x.StockStatusLabel)),
                            ComparisonOperator.Contains => products.Where(x => x.StockStatusLabel.Contains(text)),
                            ComparisonOperator.StartsWith => products.Where(x => x.StockStatusLabel.StartsWith(text)),
                            _ => products.Where(x => x.StockStatusLabel.EndsWith(text))
                        };
                    }
                }
                queryPayload.Filter?.Conditions.RemoveAll(stockConditions.Contains);

                if (sortByAvailability)
                {
                    // the availability: the available quantity (GetAvailability)
                    products = queryPayload.Sort!.Ascending
                        ? products.OrderBy(x => x.Quantity) //.ThenByDescending(x => x.Product.CreateDate)
                        : products.OrderByDescending(x => x.Quantity); //.ThenByDescending(x => x.Product.CreateDate)

                    // the custom stock status (its label, as GetStockStatus shows it), then the availability
                    //products = queryPayload.Sort!.Ascending
                    //    ? products.OrderBy(x => x.StockStatusLabel).ThenBy(x => x.Quantity)
                    //    : products.OrderByDescending(x => x.StockStatusLabel).ThenByDescending(x => x.Quantity);
                    queryPayload.Sort = null;
                }

                query = products.Select(x => x.Product);
            }

            if (queryPayload.Sort != null && queryPayload.Sort.SortBy.Equals("ProductPrices.UnitPrice", StringComparison.OrdinalIgnoreCase))
            {
                query = queryPayload.Sort.Ascending
                    ? query.OrderBy(p => p.ProductPrices.Where(pp => pp.PriceListId == priceListId).Select(pp => pp.UnitPrice).FirstOrDefault())
                    : query.OrderByDescending(p => p.ProductPrices.Where(pp => pp.PriceListId == priceListId).Select(pp => pp.UnitPrice).FirstOrDefault());
                queryPayload.Sort = null;
            }

            return query;

            // the value of the condition (Equals, NotEquals, Contains, StartsWith, EndsWith) or its values (In, NotIn)
            static List<string> Values(FilterCondition condition)
            {
                var values = condition.Operator is ComparisonOperator.In or ComparisonOperator.NotIn ? condition.Values : [condition.Value ?? string.Empty];
                if (values.Count == 0 || values.Any(string.IsNullOrEmpty))
                    throw new CustomInvalidDataException($"Filtering by '{condition.Field}' with {condition.Operator} requires a value in '{(condition.Operator is ComparisonOperator.In or ComparisonOperator.NotIn ? "Values" : "Value")}' property.");

                return values;
            }
        }
```

البناء على الحزم الحقيقية مع `Nullable` مفعّل: **0 تحذير، 0 خطأ** في الدالة.

### 7.4 الشرح سطراً بسطر

| السطر | ماذا يفعل | لماذا هكذا |
|---|---|---|
| `stockConditions = …Where(…IsAvailable… \|\| …StockStatusLabel…)` | شروط المستوى الأعلى على الحقلين، بلا حساسية لحالة الأحرف | المقارنة `Equals(…, OrdinalIgnoreCase)` هي مقارنة المؤلف في فلتر قائمة الأسعار. و`nameof(ProductDto.…)` يربط الاسم بحقل الـ DTO عند الترجمة |
| `sortByAvailability = Sort != null && Sort.SortBy.Equals(nameof(ProductDto.IsAvailable), …)` | هل الترتيب المطلوب هو التوفّر؟ | صيغة شرط ترتيب السعر حرفياً |
| `if (stockConditions.Count > 0 \|\| sortByAvailability)` | **البوابة:** بدونها لا يُبنى شيء | طلبات المؤلف (14 من 14) تعطي SQL مطابقاً حرفياً |
| `warehouseId == Guid.Empty → null` | | السطر الأول من `GetAvailability` |
| الإسقاط الأول `Quantity = …Sum((long?)OnHand - Committed) ?? 0` | الكمية | **صيغة `GetAvailability` حرفياً، ونوعها `decimal`** كما عند المؤلف (القسم 3.1.1) |
| الإسقاط الثاني `IsAvailable` و`StockStatusLabel` | ما يعرضه `GetStockStatus` | `IsAvailable = Quantity > 0` يعني `decimal > 0`، تماماً كـ `availableQuantity > 0`. والنص يقارن `(int)Quantity` كما يقارن المؤلف `currentQuantity = (int)availableQuantity`، بتقنية المسودة `dbContext.CustomStockStatuses.Where(…)` |
| `if (… Logic == OR && (Conditions.Count > 1 \|\| Groups.Count > 0))` → 400 | **القاعدة الوحيدة الجديدة** | كل شرط منّا `Where` مستقل، أي AND. تحت OR مع شروط أخرى يتغيّر المعنى، فنرفض بوضوح. شرط واحد تحت OR مسموح، لأن OR بشرط واحد هو الشرط نفسه |
| `IsAvailable`: فحص العملية أولاً | `Equals`، `NotEquals`، `In`، `NotIn` فقط | أي عملية أخرى (`Contains`، `GreaterThan`، `IsNull`…) لا معنى لها على متوفر/غير متوفر، فترجع 400 برسالة واضحة |
| `Values(condition)` ثم `bool.TryParse` | القيمة (`Value`) أو القيم (`Values` لـ `In`/`NotIn`)، و`true`/`false` بلا حساسية لحالة الأحرف | قيمة فارغة أو غير صالحة ترجع 400 |
| `include` و`isAvailable.Count == 2` | `true` و`false` معاً تعني كل المنتجات (`In`) أو لا شيء (`NotIn`) | حالة حدّية صريحة |
| `products.Where(x => x.IsAvailable)` / `!x.IsAvailable` | الشرط نفسه | يتحوّل في SQL إلى `Sum > 0` أو `Sum <= 0` مباشرة، وهو الأخفّ |
| `StockStatusLabel`: فحص العملية أولاً | `Equals`، `NotEquals`، `In`، `NotIn`، `Contains`، `StartsWith`، `EndsWith` | `GreaterThan`/`LessThan` على نص، و`IsNull`/`NotNull` (النص لا يكون فارغاً أبداً)، ترجع 400 |
| `labels.Contains(x.StockStatusLabel)` | `Equals`/`In`: أحد النصوص. `NotEquals`/`NotIn`: ليس أحدها | قائمة كمعامل، مثل `productIds.Contains(…)` عند المؤلف |
| `x.StockStatusLabel.Contains(text)` / `StartsWith` / `EndsWith` | بحث نصي | EF يرسل القيمة كمعامل `LIKE @… ESCAPE`، فالرموز `%` و`_` حرفية |
| `RemoveAll(stockConditions.Contains)` | حذف الشروط المعالَجة | وإلا رفضها `GenerateQuery` |
| `OrderBy(x => x.Quantity)` / `OrderByDescending` | **ترتيب التوفّر** | كـ `ThenBy` المسودة. تنازلياً الأكثر توفراً أولاً |
| `//.ThenByDescending(x => x.Product.CreateDate)` | فاصل التعادل، معلّق | القسم 9.1 |
| النسخة المعلّقة | الحالة (النص) ثم الكمية | المسودة مصحّحة (القسم 8.4) |
| `queryPayload.Sort = null` | | نمط ترتيب السعر |
| `query = products.Select(x => x.Product)` | العودة إلى `Product` | يكمل `GenerateQuery` كالمعتاد |
| كتلة `ProductPrices.UnitPrice` | ترتيب السعر حرفياً | بعد فلتر المخزون، فيرتّب النتيجة المفلترة |
| `static List<string> Values(FilterCondition condition)` | دالة محلية صغيرة منّا | قيمة الشرط أو قيمه، و400 إن كانت فارغة |

**تعدّد الشروط على الحقل نفسه:** كل شرط `Where` مستقل، أي AND. لذلك `Equals JLoka-01` + `Equals JLoka-02` لا يعطي شيئاً، **ولعدة قيم يُرسَل `In`**. هذا يختلف عن قاعدة الحزمة لحقولها (تضمّ تكرار العملية نفسها بـ OR). إن كانت الواجهة ترسل الاختيار المتعدد كشروط `Equals` متكررة، أضيف ضمّها في قائمة واحدة (سطران).

### 7.5 SQL الناتج: لا يدخل إلا ما يُطلب

- **`Sort.SortBy=IsAvailable` وحده:** المجموع فقط، في `SELECT` و`ORDER BY`، دون جدول الحالات.
- **`IsAvailable Equals true`:**

  ```sql
  WHERE … AND COALESCE((SELECT COALESCE(SUM(CAST(CAST([p0].[OnHand] AS bigint) AS DECIMAL(18,2)) - [p0].[Committed]), 0.0)
                        FROM [ProductAvailabilities] AS [p0] WHERE [p].[Id] = [p0].[ProductId]), 0.0) > 0.0
  ```

  لاحظ `CAST(… AS bigint)` ثم `AS DECIMAL(18,2)`: هذا هو `(long?)` المؤلف، يقطع كسر `OnHand` ثم يعود `decimal` قبل الطرح.
- **`StockStatusLabel Contains JLoka`:**

  ```sql
  AND COALESCE((SELECT TOP(1) [c].[Label] FROM [CustomStockStatuses] AS [c]
                WHERE [c].[QuantityFrom] <= CAST(<المجموع> AS int) AND [c].[QuantityTo] >= CAST(<المجموع> AS int)),
               CASE WHEN <المجموع> > 0.0 THEN N'Available' WHEN [p].[AllowBackorder] = CAST(1 AS bit) THEN N'Back order' ELSE N'Out of Stock' END)
      LIKE @__text_0_contains ESCAPE N'\'
  ```

### 7.6 السرعة (LocalDB؛ عدّ + صفحة 50؛ الوسيط من 5)

| الطلب | 2,000 منتج | 20,000 منتج |
|---|---|---|
| الافتراضي (المؤلف = المقترح) | 12–13 ms | 79–80 ms |
| ترتيب السعر (المؤلف = المقترح) | 44–52 ms | 409–447 ms |
| `IsAvailable Equals true` | 41 ms | 249 ms |
| `Sort IsAvailable desc` | 52 ms | 499 ms |
| `Sort IsAvailable` + `IsAvailable Equals true` | 97 ms | 915 ms |
| `StockStatusLabel Contains JLoka` | 321 ms | 2,561 ms |
| `StockStatusLabel In [Back order, Out of Stock]` | 418 ms | 2,064 ms |
| `Sort IsAvailable` + `StockStatusLabel Contains` | 496 ms | 4,078 ms |
| الترتيب المعلّق (الحالة ثم التوفّر) لو فُعّل | 400 ms | 4,210 ms |

- **ما يخص `IsAvailable`** بمستوى ترتيب السعر الموجود.
- **ما يقرأ نص الحالة أثقل** بسبب تقنية المسودة، إذ يُحسب المجموع داخل استعلام الحالات.
- **البديل الأسرع** (`LEFT JOIN` إلى `GROUP BY`، بين 0.3 و0.45 ثانية) ليس من تقنيات المؤلف، فتُرك.

### 7.7 لماذا `IsAvailable` للترتيب، و`StockStatusLabel` للنص، ولا حقل جديد

| الخيار | التقييم |
|---|---|
| ترتيب `IsAvailable` بالقيمة المنطقية فقط | لا يكفي: مجموعتان بلا ترتيب داخلهما |
| **ترتيب `IsAvailable` بالكمية** | الاسم حقل الاستجابة نفسه، والمعنى التوفّر الكامل، كمسودة المؤلف |
| حقل جديد `StockStatus` | يكرّر `stockStatusLabel` ويناقضه، ويغيّر كل استجابات `ProductDto` وعقد الحزمة، ولا يضيف قدرة |
| **فلترة `IsAvailable`** | متوفر / غير متوفر كما في الاستجابة |
| **فلترة `StockStatusLabel`** | نص الحالة كما يُعرض، بعمليات النص كلها |

### 7.8 فلتر وحده، ترتيب وحده، أو الاثنان معاً

| الطلب | في الدالة | ثم `GenerateQuery` | حالات |
|---|---|---|---|
| لا شيء من حقلينا | البوابة مغلقة، ثم ترتيب السعر إن طُلب | الطلب كله كما عند المؤلف | الطلبات الـ 14 |
| فلتر حقلينا فقط | الإسقاط، ثم `Where`، ثم حذف الشروط | الفلاتر الأخرى والترتيب والبحث والصفحة | F*، L*، C1، C2 |
| ترتيب `IsAvailable` فقط | المجموع، ثم `OrderBy`، ثم `Sort = null` | الفلاتر والبحث والصفحة | S1، S2، S6، S7 |
| فلتر حقلينا + ترتيب `IsAvailable` | الفلتر، ثم الترتيب على النتيجة | الباقي | S5 |
| فلتر حقلينا + ترتيب السعر | الفلتر، ثم ترتيب السعر المنقول | الباقي | X1، X3 |
| ترتيب `IsAvailable` + فلتر السعر | شرط قائمة الأسعار (كود المؤلف) يُضاف قبل، وترتيبنا يعمل | فلتر السعر | X2 |
| فلتر حقلينا + ترتيب بحقل للمؤلف | الفلتر فقط | `SortBuilder` يرتّب | C1 |
| ترتيب `StockStatusLabel` | ليس من ترتيبات المقترح | 400 كما اليوم | S3، S4 |

---

## 8. كل الحالات المغطّاة مع التجربة (`GET api/Products/shopping-products?pageSize=200&…`)

### 8.1 العمليات المدعومة ورسائلها (كلها `CustomInvalidDataException`، أي 400)

| العملية | `IsAvailable` | `StockStatusLabel` |
|---|---|---|
| `Equals` / `NotEquals` | ✔ (`true`/`false`) | ✔ (حالة الأحرف حسب الـ collation) |
| `In` / `NotIn` (`Values`) | ✔ | ✔ |
| `Contains` / `StartsWith` / `EndsWith` | ✘ `IsAvailable can be filtered only with Equals, NotEquals, In or NotIn.` | ✔ |
| `GreaterThan` … / `IsNull` / `NotNull` | ✘ (الرسالة نفسها) | ✘ `StockStatusLabel can be filtered only with Equals, NotEquals, In, NotIn, Contains, StartsWith or EndsWith.` |
| بلا قيمة | ✘ `Filtering by 'IsAvailable' with Equals requires a value in 'Value' property.` | ✘ (الرسالة نفسها) |
| قيمة غير صالحة | ✘ `Invalid IsAvailable value: 'yes'. Allowed values are: true, false.` | — |
| `Filter.Logic=OR` مع شروط أخرى | ✘ `Filtering by '…' is supported only with AND logic.` | ✘ (الرسالة نفسها) |

### 8.2 الحالات (قبل = كود المؤلف، بعد = المقترح)

| # | الطلب (معاملات الرابط) | قبل | بعد |
|---|---|---|---|
| F1 | `Filter.Conditions[0].Field=IsAvailable&…Operator=Equals&…Value=true` | 400 `not allowed` | 14 منتجاً متوفراً |
| F2 | `…IsAvailable Equals false` | 400 | 5 |
| F3 | `…isavailable Equals True` | 400 | 14 |
| F4 | `…IsAvailable NotEquals true` | 400 | 5 |
| F5 | `…IsAvailable In [true,false]` | 400 | 19 |
| F6 | `…IsAvailable NotIn [true]` | 400 | 5 |
| F7 | F1 + `warehouseId=W1` | 400 | 14 بكميات W1 |
| F8 | `IsAvailable Equals false` + `AllowBackorder Equals true` | 400 | 3 (Back order) |
| F9 | `IsAvailable Equals true` + `IsAvailable Equals false` | 400 | 0 (AND) |
| L1/L2 | `StockStatusLabel Equals JLoka-01` / `jloka-01` | 400 | 5 / 5 |
| L3 | `…Equals Available` | 400 | 3 (النص الافتراضي فقط) |
| L4 | `…NotEquals Available` | 400 | 16 |
| L5 | `…Contains JLoka` | 400 | 6 |
| L6 | `…Contains available` | 400 | 8 |
| L7 | `…Contains order` | 400 | 3 |
| L8 | `…StartsWith Out` | 400 | 2 |
| L9 | `…EndsWith 01` | 400 | 5 |
| L10 | `…In [Back order, Out of Stock]` | 400 | 5 |
| L11 | `…NotIn [Available, Available brian 1]` | 400 | 11 |
| L12 | `Equals JLoka-01` + `Equals JLoka-02` | 400 | 0 (AND؛ لعدة قيم أرسل `In`) |
| L13 | `Contains JLoka` + `NotEquals JLoka-02` | 400 | 5 |
| L14 | `…Contains %` | 400 | 0 (`%` حرفية) |
| L15/L16 | `IsNull` / `NotNull` | 400 | 400 (عملية غير مدعومة) |
| L17 | `Equals Available brian 1` + `warehouseId=W1` | 400 | 6 |
| C1 | `StockStatusLabel Contains JLoka` + `Name Contains Alex` | 400 | 2 |
| C2 | `IsAvailable Equals true` + `Search.Value=Item` | 400 | 3 |
| C3 | `Filter.Logic=OR` بشرطين من حقلينا | 400 | 400 (AND فقط) |
| C4 | `Filter.Groups[0]` فيه `IsAvailable` | 400 | 400 كما قبل (المجموعات المتداخلة خارج النطاق) |
| C5 | `ProductPrices.UnitPrice GreaterThan 10` + `IsAvailable Equals true` | 400 | 7 |
| S1 | `Sort.SortBy=IsAvailable&Sort.Ascending=false` | 400 `Sorting … not supported` | 19 بالكمية تنازلياً: 1000, 200, 80, 63, 50, 30, 12, 7, 5, 4, 3, 2, 1, 0.5, 0, 0, 0, −3, −3 |
| S2 | `…Ascending=true` | 400 | 19 تصاعدياً |
| S3 | `Sort.SortBy=StockStatusLabel` | 400 | 400 كما قبل |
| S4 | `Sort.SortBy=StockStatusLabel` + `IsAvailable Equals true` | 400 | 400 `Sorting by 'StockStatusLabel' is not supported.` |
| S5 | `Sort.SortBy=isavailable` desc + `StockStatusLabel Contains JLoka` | 400 | 6: 200, 80, 63, 50, 30, 12 |
| S6 | S1 + `warehouseId=W1` | 400 | 19 بكميات W1 |
| S7 | S1 + `pageSize=5&pageNumber=2` | 400 | العناصر 6..10، و`TotalCount` = 19 |
| X1 | `Sort.SortBy=ProductPrices.UnitPrice` asc + `IsAvailable Equals true` | 400 | 14 بسعر القائمة: 2, 3 … 40 |
| X2 | `Sort.SortBy=IsAvailable` desc + `ProductPrices.UnitPrice GreaterThan 10` | 400 | 9 بالكمية تنازلياً |
| X3 | `Sort.SortBy=ProductPrices.UnitPrice` desc + `StockStatusLabel In [JLoka-01, Back order]` | 400 | 8 بالسعر تنازلياً |
| E1 | `IsAvailable Equals yes` | 400 | 400 `Invalid IsAvailable value: 'yes'…` |
| E2/E12 | `IsAvailable Equals` / `NotEquals` بلا `Value` | 400 | 400 `…requires a value in 'Value' property.` |
| E3/E4/E5 | `IsAvailable` بـ `GreaterThan` / `Contains` / `IsNull` | 400 | 400 `IsAvailable can be filtered only with Equals, NotEquals, In or NotIn.` |
| E6 | `IsAvailable In` بلا `Values` | 400 | 400 `…requires a value in 'Values' property.` |
| E7 | `StockStatusLabel GreaterThan A` | 400 | 400 `StockStatusLabel can be filtered only with …` |
| E8/E11 | `StockStatusLabel Contains` / `StartsWith` بلا `Value` | 400 | 400 `…requires a value…` |
| E9 | `Filter.Logic=OR`: حقلنا + `Name` | 400 | 400 `…supported only with AND logic.` |
| E10 | `StockStatusColor Equals …` | 400 | 400 كما قبل |

### 8.3 ترتيب `IsAvailable` تنازلياً (S1) — هو ترتيب خرجك نفسه

| # | المنتج | الكمية | isAvailable | stockStatusLabel |
|---|---|---|---|---|
| 1 | Jafar-Product-03 | 1000 | ✔ | Available |
| 2 | Loka-01 | 200 | ✔ | JLoka-02 |
| 3–7 | Test-MetaField، Multi Warehouse، Plug، Alex Red، Alex | 80…12 | ✔ | JLoka-01 |
| 8 | matt test child 2 | 7 | ✔ | Available |
| 9–13 | iphone caver، OnHand 4.5، Nitrile، Paintbrush، Scrub Suit | 5…1 | ✔ | Available brian 1 |
| 14 | Half Unit Item | 0.5 | ✔ | Available |
| 15–17 | Zero Stock Backorder، Back Order No Rows، Out Of Stock No Rows | 0 | ✘ | Back order / Back order / Out of Stock |
| 18–19 | Committed Exceeds، Oversold | −3 | ✘ | Back order / Out of Stock |

ترتيب التوفّر لا يجمع النصوص في كتل، لأن `Available` الافتراضي يملأ الفجوات بين المديات (القسم 4). من يريد الكتل يفعّل النسخة المعلّقة.

### 8.4 النسخة المعلّقة لو فُعّلت (الحالة ثم التوفّر)

**تنازلياً:**

| النص | المنتجات بالكمية تنازلياً |
|---|---|
| Out of Stock | Out Of Stock No Rows (0)، Oversold (−3) |
| JLoka-02 | Loka-01 (200) |
| JLoka-01 | 80، 63، 50، 30، 12 |
| Back order | 0، 0، −3 |
| Available brian 1 | 5، 4، 3، 2، 1 |
| Available | Jafar (1000)، matt (7)، Half Unit (0.5) |

**تصاعدياً:** العكس. Available (0.5، 7، 1000)، ثم Available brian 1، ثم Back order، ثم JLoka-01، ثم JLoka-02، ثم Out of Stock.

كل نص كتلة واحدة، وترتيب الكتل أبجدي بنص الحالة. هذا ما قصدته المسودة. والتصحيح الوحيد ترتيب بالنص بدل الكيان (GUID).

### 8.5 مثال طلب كامل وصداه

```json
// GET …?Sort.SortBy=IsAvailable&Sort.Ascending=false&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=Contains&Filter.Conditions[0].Value=JLoka
{ "sort": { "sortBy": "IsAvailable", "ascending": false },
  "filter": { "logic": "AND", "conditions": [ { "field": "StockStatusLabel", "operator": "Contains", "value": "JLoka" } ] } }
```

الصدى بعد المعالجة. الشرط والترتيب المعالَجان حُذفا، كما يحذف المؤلف ترتيب السعر:

```json
{ "filter": { "logic": "AND", "conditions": [], "groups": [] }, "totalCount": 6 }
```

---

## 9. `CreateDate`: لماذا وضعته، ولماذا هو معلّق الآن

### 9.1 في ترتيب المنتجات (فاصل التعادل)

- **ما يفعله:** حين يتساوى منتجان في المفاتيح، يأتي الأحدث أولاً، وهو ترتيب القائمة الافتراضي.
- **مثال من البيانات:** `Zero Stock Backorder` و`Back Order No Rows` كلاهما `Back order` بكمية 0.
  - بدون فاصل يختار SQL Server ترتيبهما.
  - معه يأتي الأحدث إنشاءً أولاً.
- **هل هو نمط المؤلف؟ لا.** `SortBuilder` يرتب بمفتاح واحد دائماً، وترتيب السعر كذلك.
- **ما قسناه سابقاً:** على 1,500 منتج و1,119 في تعادل، لم يظهر أي تكرار أو فقدان بين الصفحات، معه أو بدونه.
- **التوصية:** يبقى معلّقاً كما طلبت. تفعيله إضافة مفتاح واحد في سطر الترتيب.

### 9.2 في نص الحالة (`OrderByDescending(c => c.CreateDate)`)

- **ما يفعله:** حين يحوي **أكثر من مدى** الكمية نفسها، يختار الأحدث. هذا ما يفعله `GetStockStatus`، لأن `Select` يرتّب `CreateDate desc`.
- **متى يحدث التداخل؟** `IsValidRecord` يمنعه. يحدث فقط بإدخال مباشر في القاعدة، أو بحفظين متزامنين، إذ لا قيد في القاعدة.
- **مثال مُثبَت (القسم 4 من التقرير):** أضفتُ حالة `JLoka-01 promo` بمدى 25..60، أحدث من `JLoka-01` ذات المدى 10..99.
  - `GetStockStatus` يعرض `JLoka-01 promo` لمنتجي Plug (50) وAlex Red (30).
  - **بدون** `OrderByDescending` أعاد الفلتر `StockStatusLabel Contains promo` في تشغيل سابق **لا شيء**، وفي التشغيل الأخير المنتجين.
  - **النتيجة إذن يختارها SQL Server بلا قاعدة** في هذه الحالة النادرة، فقد لا تطابق ما يُعرض.
- **التوصية:** يبقى معلّقاً كما طلبت حتى تجرّب. لكن تفعيله يعيد التطابق التام، وكلفته صفر تقريباً لأن الجدول صغير.

---

## 10. حدود معروفة ونقاط لا أغيّرها (للأمانة)

1. **`GetStockStatus` يقرأ أحدث 50 حالة فقط** (`PageSize` الافتراضي)، بينما نقرأ كل الحالات. لا فرق إلا إن تجاوز عدد الحالات 50.
2. **قاعدة الحالة مكرّرة في SQL:** صيغة `GetAvailability` وقاعدة `GetStockStatus`/`GetDefaultStockLabel` (النصوص الثلاثة). لا مفرّ من ذلك، لأنها في C# لكل منتج. إن غيّرها المؤلف يجب تغييرها في `ApplyTransactionFiltersAndSort`.
3. **الشروط على حقلينا تُضمّ بـ AND**، ولعدة قيم يُرسَل `In`/`NotIn` (القسم 7.4). `Filter.Logic=OR` مع شروط أخرى يرجع 400.
4. **المجموعات المتداخلة (`Filter.Groups`) لحقلينا** خارج النطاق، كما ينظر فلتر قائمة الأسعار للمستوى الأعلى فقط. يرفضها المؤلف بخطئه المعتاد (400).
5. **التداخل بين المديات:** القسم 9.2.
6. **فلاتر العرض حسب الدور (`ApplyActionViewFilterAsync`)** تطبَّق على `CustomStockStatusService.Select` إن أُعدّت. استعلامنا يقرأ الجدول مباشرة، كما تفعل المسودة.
7. **حساسية حالة الأحرف** في النص حسب الـ collation.
8. **السرعة:** ما يقرأ نص الحالة أبطأ بتقنية المسودة على الكتالوجات الكبيرة (القسم 7.6).
9. **نقاط في كود المؤلف رأيتها ولم أمسّها:**
   - فلتر مدى السعر (شرطان) يصبح `Any` منفصلاً عن شرط قائمة الأسعار.
   - شرط قائمة الأسعار يُضمّ بـ OR تحت `Filter.Logic=OR`.
   - `TotalCount` في الاستجابة هو عدد عناصر الصفحة.
   - الحلقة تكلّف 4 رحلات أو أكثر إلى القاعدة لكل منتج.
   - `new Guid(warehouseId)` يعطي 500 لقيمة غير صالحة.
10. **النشر:** التعديل داخل Products API فقط. لا تغيير في `ProductDto` ولا في عقد الحزمة.

---

## 11. بيانات تؤكد التحليل على قاعدتك (اختياري، قراءة فقط)

إن أردت أن أطابق الشرح على بياناتك الحقيقية بدل المديات المفترضة، شغّل هذا على قاعدة **اختبار** (ليس الإنتاج) وأرسل الناتج:

```sql
-- 1) الحالات المخصّصة
SELECT Label, QuantityFrom, QuantityTo, ColorCode, CreateDate FROM CustomStockStatuses ORDER BY QuantityFrom;

-- 2) الكمية كما يحسبها GetAvailability لمنتجات خرجك
SELECT p.Name, p.AllowBackorder,
       COALESCE(SUM(CAST(CAST(pa.OnHand AS bigint) AS DECIMAL(18,2)) - pa.Committed), 0) AS Available
FROM Products p LEFT JOIN ProductAvailabilities pa ON pa.ProductId = p.Id
WHERE p.Name IN (N'Jafar-Product-03', N'Loka-01', N'Test-MetaField', N'Plug', N'matt test child 2', N'iphone caver')
GROUP BY p.Name, p.AllowBackorder ORDER BY Available DESC;
```

---

## 12. ماذا يحدث بعد "طبق"

1. **نسخة احتياطية** في `_backups\IC-2132-TransactionFiltersAndSort-BEFORE-<الوقت>` لملف `ProductService.cs` (`4081805d`)، مع `restore.ps1`.
2. **`ProductService.cs`** (‏BOM + CRLF محفوظان):
   - الأسطر 220–236 (ترتيب السعر ومسودة الترتيب) يحلّ محلها سطر الاستدعاء مع تعليقه.
   - إضافة `ApplyTransactionFiltersAndSort` بعد `GetProductsForTransaction`.
   - لا `using` جديد.
3. **`ProductDto.cs` لا يُمسّ.**
4. **تحقّق Roslyn:**
   - لم يتغيّر إلا `GetProductsForTransaction` والدالة الجديدة.
   - فلتر قائمة الأسعار بقي حرفياً، وترتيب السعر داخل الدالة مطابق للأصل.
   - نص الدالة مطابق لما نجح في الاختبار 76/76.
   - `GetStockStatus` و`GetAvailability` و`GetDefaultStockLabel` لم تُمسّ.
5. **عرض الـ diff والبصمات (MD5).** البناء والتجربة عندك.
