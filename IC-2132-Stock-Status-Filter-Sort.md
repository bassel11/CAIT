# IC-2132 – الفلترة والترتيب حسب حالة المخزون في `shopping-products`

> **الحالة:** تحليل ومقترح مُختبَر. **لم يُطبَّق أي سطر من المقترح.** الملفات الآن هي الأصلية (انظر القسم 1).
> **الإثبات (التصميم المراجَع):** 76 فحصاً من 76 نجحت على **النموذج الحقيقي**: قاعدة LocalDB مؤقتة أُنشئت من `ITM.Core.Organization.Database 26.1008.2704`، والاستعلام يمرّ في `QueryHelper.GenerateQuery` و`FilterBuilder` الحقيقيين (`ITM.Core.Organization 26.1008.2718`) مع `ProductDto` الحقيقي (`ITM.Core.Products 26.1009.2721`). التقرير الكامل لكل حالة مع الخرج موجود في الملحق [IC-2132-Stock-Status-Test-Report.md](IC-2132-Stock-Status-Test-Report.md).

---

## 0. الخلاصة في عشرة أسطر

1. **أعدتُ الملفات إلى ما كانت عليه قبل تعديلاتنا** بايتاً ببايت: `ProductService.cs` = `4081805d…` و`ProductDto.cs` = `19326988…`. حذفتُ `Helper/ExpressionExtensions.cs`، ولم يعد هناك أي enum.
2. ⚠️ الملف المُسترجَع **ما زال يحوي مقطع التجربة الذي وضعتَه في الأسطر 228–236**، وهذا المقطع يُبطل كل ترتيب ترسله الواجهة. **لا تنشر الملف المُسترجَع كما هو** (التفاصيل في القسم 1.2).
3. **لا enum ولا حقل جديد في `ProductDto`.** الواجهة تستلم أصلاً `isAvailable` و`stockStatusLabel`، فتفلتر وترتب بهذين الاسمين نفسيهما.
4. **لا معالج فلترة أو ترتيب عام.** تخلّيتُ عن `CustomQuery`، فقد كان يعيد كتابة `FilterBuilder` (القسم 7.0). ما يُضاف الآن:
   - صف صغير `Helper/ProductStockStatus.cs`.
   - دالة خاصة `ApplyStockStatus` تُستدعى بسطر واحد في `GetProductsForTransaction`، وتعمل فقط حين يذكر الطلب `IsAvailable` أو `StockStatusLabel`.
5. **كود المؤلف لا يُمسّ:** فلتر قائمة الأسعار، وترتيب السعر (IC-2064)، و`GetStockStatus`، و`GenerateQuery`، والحلقة، و`ProductDto`، كلها كما هي. يُحذف فقط مقطع التجربة. كل طلب لا يذكر الحقلين يعطي **نص SQL مطابقاً حرفياً** (14 من 14).
6. **باني المؤلف نفسه، `FilterBuilder.BuildPredicate`، يبني شروط الحقلين.** لذلك يتصرّف `IsAvailable` تماماً مثل `AllowBackorder`، و`StockStatusLabel` تماماً مثل `Name`، حتى في رسائل الخطأ والحالات الحدّية (القسم 8.1).
7. **الكمية تُحسب مرة واحدة لكل المنتجات** (`LEFT JOIN` إلى `GROUP BY` واحد)، فقط عند طلب الحقلين: من 163 إلى 470 ms على 20,000 منتج، بينما يأخذ ترتيب السعر الموجود 491 ms.
8. **`CreateDate` مُعلَّق** في الموضعين كما طلبت. القسم 9 يشرح بالأمثلة لماذا وُضع وما الذي يتغيّر بدونه.
9. **سبب ظهور `Available… JLoka… Available`:** الترتيب كان بالكمية، والحالات المخصّصة مدياتٌ على الكمية بينها فجوات يملؤها النص الافتراضي `Available`. الترتيب الجديد بـ `StockStatusLabel` يجعل كل نص كتلة واحدة (القسم 4).
10. **التطبيق ينتظر كلمة "طبق"** (القسم 12).

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

### 1.2 ⚠️ مقطع التجربة في الأسطر 228–236 من الملف المُسترجَع

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

هذا المقطع كان في الملف قبل أول تعديل لي، لذلك عاد مع الاسترجاع الحرفي. ما يفعله فعلياً، **مُثبَت على النموذج الحقيقي** (القسم 2 من التقرير):

| السلوك | الدليل |
|---|---|
| يعمل **دائماً**، حتى بلا أي طلب ترتيب، فيغيّر ترتيب القائمة الافتراضي (`CreateDate desc`) | الطلب الافتراضي: ترتيب المؤلف `Zero Stock Backorder, Oversold Item, …` وترتيب الملف المُسترجَع `Committed Exceeds Item, Oversold Item, …` |
| `OrderBy(... .FirstOrDefault())` على كيان **يرتّب بمعرّف الحالة (GUID)**، فلا معنى للترتيب | SQL: `ORDER BY (SELECT TOP(1) [c].[Id] FROM [CustomStockStatuses] …)` |
| يُلغي **كل** ترتيب ترسله الواجهة (`Sort = null`)، فلا يعود `Sort.SortBy=Name` يعمل | `Sort Name asc`: المؤلف `Alex Vando Shirt, …` والملف المُسترجَع ترتيب GUID |
| يُلغي ترتيب السعر (IC-2064)، لأن `OrderBy` الثاني يمحو الأول | `Sort ProductPrices.UnitPrice asc`: النتيجتان مختلفتان |
| صيغته تختلف عن صيغة المؤلف: `OnHand - Committed` بلا `(long?)`، وبلا مستودع | SQL: `SUM([p0].[OnHand] - [p0].[Committed])` بلا `CAST … bigint` وبلا `WarehouseId` |
| يحذف الترتيب من الصدى المرسل للواجهة | echo المؤلف `"Sort":{"SortBy":"CreateDate",…}` والملف المُسترجَع بلا `Sort` |

**المقترح في القسم 7 يحذف هذا المقطع.** وإن أردتَ أن يكون الملف الآن هو الأصل **بدونه**، قل ذلك وأحذفه وحده مع نسخة احتياطية.

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
| 228–236 | مقطع التجربة | القسم 1.2 |
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

**لا يمكن الحصول على الأمرين معاً**، أي ترتيب بالكمية وكل نص في كتلة واحدة، لأن النص `Available` نفسه موزّع على مجالات غير متصلة. لذلك يعطي المقترح **ترتيبين بمعنيين واضحين**:

- `Sort.SortBy=IsAvailable`: **التوفّر**. المتوفر أولاً، ثم Back order، ثم Out of Stock، ثم الكمية. النصوص قد تتداخل، وهذا متوقع (S1).
- `Sort.SortBy=StockStatusLabel`: **الحالة**. كل نص كتلة واحدة، ثم الكمية داخل الكتلة (S3):

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
| هل نضع `[AllowFilter]`/`[AllowSorting]` على `IsAvailable`/`StockStatusLabel`؟ | **لا.** معالجتنا تسبق الحزمة فلا تحتاجها. ووضعها يجعل الحزمة في المسارات الأخرى تبحث عن عمود فتعطي 400 بعد أن "أعلنت" الدعم. المؤلف لا يضع وسماً على أي حقل محسوب في Products |

**القرار المقترح:** **لا حقل جديد ولا وسم ولا enum.** الواجهة ترسل:

- `IsAvailable` للتوفّر (فلتر وترتيب).
- `StockStatusLabel` للحالة: نص الحالة المخصّصة أو النص الافتراضي (فلتر بالنص وترتيب).
- `AllowBackorder` كما هو اليوم.

---

## 7. التصميم المقترح (المراجَع): نستعمل بُناة المؤلف ولا نعيد كتابتهم

### 7.0 لماذا تركتُ تصميم `CustomQuery` الأول

راجعتُ التصميم الأول سطراً بسطر مقابل كود الحزمة المفكوك. النتيجة أنه كان **مُحرّك فلترة وترتيب ثانياً موازياً لمحرّك المؤلف**. أعاد كتابة منطق المؤلف ولم يستدعه:

| ما أعاد التصميم الأول كتابته | مقابله عند المؤلف (في الحزمة) |
|---|---|
| تحويل كل عملية إلى تعبير (Equals/NotEquals/GreaterThan…/Contains/StartsWith/EndsWith) | `FilterBuilder.CreateComparisonExpression` وما يتبعها |
| تحويل القيمة بـ `TypeConverter` ورسائل `Failed conversion…` | `FilterBuilder.BuildConditionExpression` |
| `In` بـ `NewArrayInit` + `Contains` | `FilterBuilder.InExpression` |
| فحص `IsNull`/`NotNull` للأنواع القابلة لـ null | `FilterBuilder.CreateNullComparisonExpression` |
| دمج الشروط حسب الحقل والعملية | `QueryHelper.NormalizeFilterGroup` |
| المرور على المجموعات AND/OR | `FilterBuilder.BuildExpression` |
| ترتيب بمفاتيح عبر Reflection | `SortBuilder.ApplyOrderBy` |
| نقل فلتر قائمة الأسعار وترتيب السعر إلى داخله | كود الدالة نفسها |

لذلك اختلف عن المؤلف في سلوك ملموس:
- `NotIn` كان مدعوماً عندنا، وعند المؤلف ليس مدعوماً في المستوى الأعلى.
- رسائل خطأ مختلفة.
- `GreaterThan` على النص كان يعطي 400 عندنا، وعند المؤلف 500.

ومستقبلاً: أي تغيير يُجريه المؤلف على `FilterBuilder` لن يصل إلى حقولنا. **هذا ما يجعل المراجِع يرفضه، وأنت محق في التحفّظ.** لذلك حذفتُه.

### 7.1 المبدأ الجديد

**لا نبني أي معالج عام.** نكتب كوداً واحداً خاصاً بهذه المهمة، يعمل فقط في `GetProductsForTransaction`، وفقط حين يذكر الطلب `IsAvailable` أو `StockStatusLabel`. نعطي هذين الحقلين للمؤلف **كأنهما أعمدة**، فيبنيهما **`FilterBuilder` الخاص به نفسه**. والنتيجة أنهما يتصرفان تماماً كحقول المؤلف (مُثبَت في 8.3).

| ما نستعمله من المؤلف كما هو | كيف |
|---|---|
| `FilterBuilder.BuildPredicate<,>` (عام `public`) | يبني شروط الحقلين بكل دلالته: العمليات، والتحويل، والرسائل، والقيود |
| `[AllowFilter]` و`[AllowSorting]` | نعلن بهما الحقلين القابلين للفلترة والترتيب، كما يعلن أي DTO |
| `PropertyCache.IsFilterAllowed` / `GetAllowedSortingProperties` (عامة) | نتعرّف بها على حقولنا في الطلب بقراءة الوسوم نفسها |
| `InvalidQueryException` | للقاعدة الوحيدة التي نضيفها (OR المختلط) |
| صيغة `GetAvailability` حرفياً | `Sum((long?)OnHand - Committed) ?? 0` بالمستودع نفسه |
| قاعدة `GetStockStatus` و`GetDefaultStockLabel` | مكتوبة في SQL لأنها في C# لكل منتج. هذا لا مفرّ منه |
| نمط ترتيب السعر في الدالة نفسها | `OrderBy` صريح بثلاثي `? :` ثم `Sort = null` |
| `QueryHelper.GenerateQuery` | **لم يُمسّ**، ويُستدعى كما كان |
| فلتر قائمة الأسعار (الأسطر 178–189، من كود المؤلف) وترتيب السعر (220–226، من IC-2064) | **بقيا في مكانهما حرفياً**، دون نقل ولا تغيير |

**الشيء الوحيد المنسوخ من منطق المؤلف:** قاعدة `NormalizeFilterGroup` لتجميع شروط الحقل الواحد (≈10 أسطر). هي تشكيل للبيانات فقط، ولا تبني أي تعبير. اضطررنا إليها لأن الدالة `private` في الحزمة، ولأن وجودها يدل على أن الواجهة ترسل الحقل نفسه أكثر من مرة (اختيار متعدد)، فيجب أن تُضمّ بـ OR مثل حقول المؤلف. يمكن حذفها إن أردت، وحينها تُضمّ بـ AND.

### 7.2 الملف الجديد `Helper/ProductStockStatus.cs` (‏BOM + CRLF، ‏`internal`)

```csharp
using ITM.Core.Organization.Database.Models;

namespace ITM.Core.Products.Helper
{
    // A product with the stock status GetStockStatus shows for it, computed in the query: the package's FilterBuilder builds the conditions
    // of the shopping list on IsAvailable and StockStatusLabel here, where they are columns ([AllowFilter], [AllowSorting] as on a DTO)
    internal sealed class ProductStockStatus
    {
        public Product Product { get; set; } = null!;

        public decimal AvailableQuantity { get; set; }

        [AllowFilter]
        [AllowSorting]
        public bool IsAvailable { get; set; }

        [AllowFilter]
        [AllowSorting]
        public string StockStatusLabel { get; set; } = null!;
    }
}
```

هذا ليس DTO ولا كياناً. هو "صف" يعيش داخل SQL فقط: المنتج، وكميته، وما يعرضه `GetStockStatus`. الاسمان `IsAvailable` و`StockStatusLabel` هما اسما حقلي `ProductDto` نفسيهما، فترسلهما الواجهة كما تستلمهما.

### 7.3 التعديل في `ProductService.cs`

**(أ) `using` واحد** في رأس الملف، لأن `FilterBuilder` و`PropertyCache` في هذا الـ namespace:

```diff
 using ITM.Core.Organization.Helpers;
+using ITM.Core.Organization.Helpers.Quering;
 using ITM.Core.Organization.Models;
```

**(ب) داخل `GetProductsForTransaction`:** سطران قبل ترتيب السعر، وحذف مقطع التجربة. **لا شيء آخر يتغيّر:**

```diff
             //if (priceListId != Guid.Empty)
             //{
             //	query = query.Where(p => p.ProductPrices.Any(pp => pp.PriceListId == priceListId));
             //}
 
+            // the stock status filter and sort (IsAvailable, StockStatusLabel), before the price sort orders the query
+            query = ApplyStockStatus(query, queryPayload, parsedWarehouseId);
+
             if (queryPayload.Sort != null && queryPayload.Sort.SortBy.Equals("ProductPrices.UnitPrice", StringComparison.OrdinalIgnoreCase))
             { … كما هو … }
 
-            query = query.OrderBy(x => dbContext.CustomStockStatuses
-             … (مقطع التجربة 228-236) …
-            if(queryPayload.Sort != null)
-                queryPayload.Sort = null;
-
             query = await QueryHelper<Product, ProductDto>.GenerateQuery(queryPayload, query, _mapper, dbContext);
```

لماذا **قبل** ترتيب السعر؟ `ApplyStockStatus` يضمّ (`JOIN`) المنتجات إلى كمياتها، والترتيب الذي يسبق `JOIN` لا يضمن EF بقاءه. فإذا جاء فلتر مخزون مع ترتيب سعر، يُفلتَر أولاً ثم يرتّب كود السعر النتيجة. مُثبَت في X1 وX3.

**(ج) دالة خاصة جديدة** بعد `GetProductsForTransaction`:

```csharp
        // The stock status filter and sort of the shopping and purchasing lists, on the values GetStockStatus shows (IsAvailable, StockStatusLabel),
        // which are not columns of Product: the products are joined with their available quantity (GetAvailability, one GROUP BY for all of them)
        // as ProductStockStatus, where the package's FilterBuilder builds the conditions of the main filter on these fields (then removed from it);
        // their sort orders the query and sets Sort = null, as the price sort does. A request without them gets the query unchanged.
        private IQueryable<Product> ApplyStockStatus(IQueryable<Product> query, QueryPayload queryPayload, Guid? warehouseId)
        {
            var conditions = queryPayload.Filter?.Conditions.Where(x => !x.IsMetaField && PropertyCache.IsFilterAllowed<ProductStockStatus>(x.Field)).ToList() ?? [];
            var sortBy = queryPayload.Sort != null && !queryPayload.Sort.IsMetaField
                ? PropertyCache.GetAllowedSortingProperties<ProductStockStatus>().FirstOrDefault(x => x.Equals(queryPayload.Sort.SortBy, StringComparison.OrdinalIgnoreCase))
                : null;
            if (conditions.Count == 0 && sortBy == null)
                return query;

            if (warehouseId == Guid.Empty)
                warehouseId = null;

            // GetAvailability, for all the products at once
            var availableQuantities = dbContext.ProductAvailabilities
                .Where(pa => warehouseId == null || pa.WarehouseId == warehouseId)
                .GroupBy(pa => pa.ProductId)
                .Select(g => new { ProductId = g.Key, Quantity = g.Sum(pa => (long?)pa.OnHand - pa.Committed) });

            // GetStockStatus: available above 0; the label of the custom stock status whose range holds the quantity, else GetDefaultStockLabel
            var stock = from p in query
                        join q in availableQuantities on p.Id equals q.ProductId into quantities
                        from q in quantities.DefaultIfEmpty()
                        let quantity = q.Quantity ?? 0
                        select new ProductStockStatus
                        {
                            Product = p,
                            AvailableQuantity = quantity,
                            IsAvailable = quantity > 0,
                            StockStatusLabel = dbContext.CustomStockStatuses
                                .Where(c => c.QuantityFrom <= (int)quantity && c.QuantityTo >= (int)quantity)
                                //.OrderByDescending(c => c.CreateDate)
                                .Select(c => c.Label)
                                .FirstOrDefault() ?? (quantity > 0 ? "Available" : p.AllowBackorder ? "Back order" : "Out of Stock")
                        };

            if (conditions.Count > 0)
            {
                // applied apart from the rest of the filter: right with AND, or with OR when the filter holds nothing else
                var filter = queryPayload.Filter!;
                if (filter.Logic != LogicalOperator.AND && (conditions.Count < filter.Conditions.Count || filter.Groups.Count > 0))
                    throw new InvalidQueryException($"Filtering by '{conditions[0].Field}' is supported only with AND logic.");

                // the conditions of one field grouped as QueryHelper.NormalizeFilterGroup groups them (OR for one operator or with IsNull / NotNull,
                // else AND between the operators), then built by FilterBuilder as on any field
                var stockFilter = new FilterGroup { Logic = filter.Logic };
                foreach (var field in conditions.GroupBy(x => x.Field))
                {
                    var operators = field.GroupBy(x => x.Operator).ToList();
                    if (field.Count() == 1)
                        stockFilter.Conditions.Add(field.First());
                    else if (operators.Count == 1 || field.Any(x => x.Operator is ComparisonOperator.IsNull or ComparisonOperator.NotNull))
                        stockFilter.Groups.Add(new FilterGroup { Logic = LogicalOperator.OR, Conditions = field.ToList() });
                    else
                        stockFilter.Groups.Add(new FilterGroup
                        {
                            Logic = LogicalOperator.AND,
                            Conditions = operators.Where(x => x.Count() == 1).Select(x => x.First()).ToList(),
                            Groups = operators.Where(x => x.Count() > 1).Select(x => new FilterGroup { Logic = LogicalOperator.OR, Conditions = x.ToList() }).ToList()
                        });
                }
                stock = stock.Where(FilterBuilder.BuildPredicate<ProductStockStatus, ProductStockStatus>(stockFilter));
                filter.Conditions.RemoveAll(conditions.Contains);
            }

            // IsAvailable: available, then back order (AllowBackorder), then out of stock, then the quantity; StockStatusLabel: the label, then the quantity
            if (sortBy == nameof(ProductStockStatus.IsAvailable))
            {
                stock = queryPayload.Sort!.Ascending
                    ? stock.OrderBy(x => x.IsAvailable ? 2 : x.Product.AllowBackorder ? 1 : 0).ThenBy(x => x.AvailableQuantity)
                    : stock.OrderByDescending(x => x.IsAvailable ? 2 : x.Product.AllowBackorder ? 1 : 0).ThenByDescending(x => x.AvailableQuantity);
                queryPayload.Sort = null;
            }
            else if (sortBy == nameof(ProductStockStatus.StockStatusLabel))
            {
                stock = queryPayload.Sort!.Ascending
                    ? stock.OrderBy(x => x.StockStatusLabel).ThenBy(x => x.AvailableQuantity)
                    : stock.OrderByDescending(x => x.StockStatusLabel).ThenByDescending(x => x.AvailableQuantity);
                queryPayload.Sort = null;
            }

            return stock.Select(x => x.Product);
        }
```

- **لا يُمسّ:** `GetStockStatus`، و`GetAvailability`، و`GetDefaultStockLabel`، والحلقة، و`ProductDto`، وفلتر قائمة الأسعار، وترتيب السعر، و`GenerateQuery`.
- البناء على الحزم الحقيقية مع `Nullable` مفعّل: **0 تحذير، 0 خطأ**.

### 7.4 شرح `ApplyStockStatus` سطراً بسطر

| السطر | المعنى |
|---|---|
| `conditions = … IsFilterAllowed<ProductStockStatus>(x.Field)` | شروط المستوى الأعلى التي اسمها حقل عليه `[AllowFilter]` في `ProductStockStatus`، أي `IsAvailable` أو `StockStatusLabel` (بلا حساسية لحالة الأحرف، كالحزمة). شروط الحقول الوصفية مستثناة. المستوى الأعلى فقط، كما يفعل فلتر قائمة الأسعار |
| `sortBy = … GetAllowedSortingProperties<ProductStockStatus>()` | إن كان `SortBy` أحد الحقلين يؤخذ اسمه الأصلي، وإلا `null`. ترتيب الحقل الوصفي مستثنى |
| `if (conditions.Count == 0 && sortBy == null) return query;` | **أي طلب لا يذكر الحقلين يرجع الاستعلام نفسه.** 14 طلباً من طلبات المؤلف أعطت نص SQL مطابقاً حرفياً |
| `warehouseId == Guid.Empty → null` | كما في `GetAvailability` |
| `availableQuantities` | صيغة `GetAvailability` حرفياً، لكل المنتجات دفعة واحدة (`GROUP BY ProductId`) |
| `from p in query join … DefaultIfEmpty()` | `LEFT JOIN`: المنتج بلا صفوف مخزون كميته `?? 0`، كما عند المؤلف |
| `IsAvailable = quantity > 0` | `GetStockStatus` (‏`res.Item1`) |
| `StockStatusLabel = …` | `GetStockStatus` + `GetDefaultStockLabel`. **`OrderByDescending(CreateDate)` مُعلَّق** (القسم 9) |
| `filter.Logic != AND && …` → 400 | **القاعدة الوحيدة الجديدة.** تحت OR لا يمكن فصل حقولنا عن حقول المؤلف دون تغيير المعنى، فنرفض بوضوح بدل أن نفلتر خطأً |
| `stockFilter` | شروط الحقلين مجمّعة كما يجمّعها `NormalizeFilterGroup` (7.1) |
| `FilterBuilder.BuildPredicate<ProductStockStatus, ProductStockStatus>(stockFilter)` | **باني المؤلف** يبني الشرط على الصف كما يبنيه على أي عمود |
| `filter.Conditions.RemoveAll(conditions.Contains)` | حتى لا يرفضها `GenerateQuery` لاحقاً. هذا نمط حذف الحقل المعالَج نفسه |
| ترتيب `IsAvailable` | متوفر، ثم Back order (`AllowBackorder`)، ثم Out of Stock، ثم الكمية، بالاتجاه المطلوب. ثلاثي `? :` كترتيب السعر، ثم `Sort = null` |
| ترتيب `StockStatusLabel` | النص ثم الكمية، فكل نص كتلة واحدة (القسم 4) |
| `return stock.Select(x => x.Product);` | يعود الاستعلام إلى `Product`، ويكمل `GenerateQuery` عليه: فلتر المؤلف والبحث والعدّ والصفحة و`Expand` |

### 7.5 السرعة (20,000 منتج، عدّ + صفحة 50، الوسيط من 5)

| الطلب | المؤلف | المقترح |
|---|---|---|
| الافتراضي | 159 ms | 150 ms (الاستعلام نفسه) |
| ترتيب السعر | 491 ms | 530 ms (الاستعلام نفسه، والفرق تذبذب قياس) |
| `IsAvailable Equals true` | 400 غير مدعوم | 276 ms |
| `StockStatusLabel Contains JLoka` | 400 | 298 ms |
| `Sort IsAvailable desc` | 400 | 163 ms |
| `Sort StockStatusLabel asc` | 400 | 405 ms |
| `Sort StockStatusLabel` + `IsAvailable` | 400 | 470 ms |

الكمية تُحسب مرة واحدة (`LEFT JOIN` إلى `GROUP BY`)، ولا يحدث ذلك إلا عند طلب أحد الحقلين. لو كُتبت الكمية داخل كل تعبير لكان الزمن من 2.5 إلى 5.6 ثانية.

### 7.6 الـ SQL الناتج سطراً بسطر (`Sort.SortBy=StockStatusLabel`)

```sql
SELECT … [t0].*, [t1].* (TaxClass), [u].* (InventoryUnitOfMeasurement)        -- (8) Include الخاص بالمؤلف بعد الصفحة
FROM (
    SELECT [p].*,
           COALESCE((SELECT TOP(1) [c].[Label]                                       -- (4) النص: أول حالة مخصّصة…
                     FROM [CustomStockStatuses] AS [c]
                     WHERE [c].[QuantityFrom] <= CAST(COALESCE([t].[Quantity], 0.0) AS int)   -- …يحوي مداها (int)الكمية
                       AND [c].[QuantityTo]   >= CAST(COALESCE([t].[Quantity], 0.0) AS int)),
                    CASE WHEN COALESCE([t].[Quantity], 0.0) > 0.0 THEN N'Available'  -- (5) وإلا GetDefaultStockLabel
                         WHEN [p].[AllowBackorder] = CAST(1 AS bit) THEN N'Back order'
                         ELSE N'Out of Stock' END) AS [c],
           COALESCE([t].[Quantity], 0.0) AS [c0]                                     -- (3) الكمية، 0 بلا صفوف
    FROM [Products] AS [p]
    LEFT JOIN (                                                                      -- (2) GetAvailability لكل المنتجات مرة واحدة
        SELECT [p0].[ProductId],
               COALESCE(SUM(CAST(CAST([p0].[OnHand] AS bigint) AS DECIMAL(18,2)) - [p0].[Committed]), 0.0) AS [Quantity]
        FROM [ProductAvailabilities] AS [p0]
        -- WHERE [p0].[WarehouseId] = @__warehouseId_0   ← فقط حين يُرسل warehouseId
        GROUP BY [p0].[ProductId]
    ) AS [t] ON [p].[Id] = [t].[ProductId]
    WHERE [p].[SalesItem] = CAST(1 AS bit) AND [p].[ProductState] = 1 AND [p].[ProductVisibility] <> 20   -- (1) شروط المؤلف كما هي
    ORDER BY <النص>, <الكمية>                                                         -- (6) ترتيبنا
    OFFSET @__p_0 ROWS FETCH NEXT @__p_1 ROWS ONLY                                   -- (7) صفحة GenerateQuery
) AS [t0]
LEFT JOIN [TaxClasses] AS [t1] ON [t0].[TaxClassId] = [t1].[Id]
LEFT JOIN [UnitOfMeasurements] AS [u] ON [t0].[InventoryUnitOfMeasurementId] = [u].[Id]
ORDER BY [t0].[c], [t0].[c0]
```

فلتر `StockStatusLabel Contains JLoka` يضيف إلى `WHERE` الداخلي `AND <النص> LIKE N'%JLoka%'`، وهي صيغة `FilterBuilder` نفسها لـ `Name Contains`. وفلتر `IsAvailable Equals true` يضيف `AND COALESCE([t].[Quantity], 0.0) > 0.0`.

لا فهرس جديد ولا migration، فالفهرسان `IX_ProductAvailabilities_ProductId` و`IX_…_WarehouseId` موجودان أصلاً.

---

## 8. كل الحالات المدعومة، بلا استثناء

### 8.1 القاعدة الذهبية

**`IsAvailable` يتصرف تماماً مثل `AllowBackorder` (bool عند المؤلف)، و`StockStatusLabel` يتصرف تماماً مثل `Name` (نص عند المؤلف).** السبب أن باني المؤلف نفسه يبنيهما. وهذا يشمل حتى سلوك المؤلف في الحالات الحدّية التي لا نغيّرها:

| العملية | `IsAvailable` (= `AllowBackorder`) | `StockStatusLabel` (= `Name`) |
|---|---|---|
| `Equals` / `NotEquals` | ✔ القيم `true`/`false` | ✔ (حالة الأحرف حسب الـ collation) |
| `In` (`Values`) | ✔ | ✔ |
| `Contains` / `StartsWith` / `EndsWith` | ✘ 400 `Failed conversion from string to Boolean` | ✔ (`%` و`_` حرفية) |
| `GreaterThan` … `LessThanOrEqual` | ✘ 400 `Invalid usage of Greater Than operator.` | ✘ **500** `The binary operator GreaterThan is not defined for … String` (هذا ما يحدث اليوم لـ `Name GreaterThan`) |
| `IsNull` / `NotNull` | ✘ 400 `Operator IsNull is only supported for nullable fields.` | ✔ (`IsNull` لا يعطي شيئاً، و`NotNull` يعطي الكل) |
| `NotIn` | ✘ **500** `Argument types do not match` (هذا ما يحدث اليوم لـ `AllowBackorder NotIn`) | ✘ 400 `Operator NotIn is not implemented.` (كما لـ `Name NotIn` اليوم) |
| بلا `Value` | ✘ **500** `Argument types do not match` (كما لـ `AllowBackorder`) | `Contains`/`StartsWith` بلا قيمة لا تعطي شيئاً (كما لـ `Name`) |

الخانات **500** هي سلوك المؤلف في كل حقول bool والنص، ولم نخترعها. إن أردتها 400 فذلك تغيير في الحزمة نفسها، ليس في مهمتنا.

### 8.2 قواعد الدمج

- **شروط على الحقل نفسه**، كما يفعل `NormalizeFilterGroup`:
  - العملية نفسها تعني OR (L12).
  - عمليات مختلفة تعني AND (L13).
- **مع حقول المؤلف:** AND (F8، C1، C5).
- **`Filter.Logic=OR`:**
  - مدعوم إن كانت كل الشروط من حقلينا (C3).
  - مع حقل للمؤلف: 400 واضح (E9).
- **المجموعات المتداخلة (`Filter.Groups`):** لا نتعامل معها، كفلتر قائمة الأسعار الذي ينظر للمستوى الأعلى فقط. فيرفضها المؤلف بخطئه المعتاد `Filtering by 'IsAvailable' is not allowed.` (C4).
- **مع ترتيب السعر:** يعمل (X1، X3). **ومع فلتر السعر:** يعمل (C5، X2).
- **`warehouseId`** يغيّر الكمية، فيغيّر الفلتر والترتيب، كما يغيّر ما يُعرض (F7، L17، S6).
- **`Search.Value`:** بلا تغيير، ويُجمع مع الفلتر (C2).
- **الصفحات و`TotalCount`:** صحيحان (S7).
- **الصدى:** الشروط المعالَجة تُحذف منه، و`Sort` يصبح `null` عند ترتيبنا، كترتيب السعر.

### 8.3 الحالات مع الطلب والنتيجة

"قبل" = كود المؤلف. "بعد" = المقترح. الجداول الكاملة مع الصدى وSQL في الملحق.

| # | الطلب (`GET api/Products/shopping-products?pageSize=200&…`) | قبل | بعد |
|---|---|---|---|
| F1 | `Filter.Conditions[0].Field=IsAvailable&…Operator=Equals&…Value=true` | 400 `Filtering by 'IsAvailable' is not allowed.` | 14 منتجاً كلها `isAvailable=true` |
| F2 | `…IsAvailable Equals false` | 400 | 5 |
| F3 | `…isavailable Equals True` | 400 | 14 |
| F4 | `…IsAvailable NotEquals true` | 400 | 5 |
| F5 | `…IsAvailable In [true,false]` | 400 | 19 |
| F6 | `…IsAvailable NotIn [true]` | 400 | **500 كـ `AllowBackorder NotIn`** |
| F7 | F1 + `warehouseId=W1` | 400 | 14 بكميات W1 |
| F8 | `IsAvailable Equals false` + `AllowBackorder Equals true` | 400 | 3 (Back order) |
| F9 | `IsAvailable Equals true` + `IsAvailable Equals false` | 400 | 19 (OR) |
| L1/L2 | `StockStatusLabel Equals JLoka-01` / `jloka-01` | 400 | 5 / 5 |
| L3 | `…Equals Available` | 400 | 3: Half Unit، matt، Jafar (النص الافتراضي فقط) |
| L4 | `…NotEquals Available` | 400 | 16 |
| L5 | `…Contains JLoka` | 400 | 6 |
| L6 | `…Contains available` | 400 | 8 |
| L7 | `…Contains order` | 400 | 3 Back order |
| L8 | `…StartsWith Out` | 400 | 2 Out of Stock |
| L9 | `…EndsWith 01` | 400 | 5 |
| L10 | `…In [Back order, Out of Stock]` | 400 | 5 |
| L11 | `…NotIn [Available, Available brian 1]` | 400 | **400 `Operator NotIn is not implemented.` كـ `Name NotIn`** (البديل: `NotEquals` أو `In`) |
| L12 | `Equals JLoka-01` + `Equals JLoka-02` | 400 | 6 (OR) |
| L13 | `Contains JLoka` + `NotEquals JLoka-02` | 400 | 5 (AND) |
| L14 | `…Contains %` | 400 | 0 |
| L15/L16 | `IsNull` / `NotNull` | 400 | 0 / 19 |
| L17 | `Equals Available brian 1` + `warehouseId=W1` | 400 | 6 |
| C1 | `StockStatusLabel Contains JLoka` + `Name Contains Alex` | 400 | 2 |
| C2 | `IsAvailable Equals true` + `Search.Value=Item` | 400 | 3 |
| C3 | `Filter.Logic=OR`: `StockStatusLabel Equals Back order` OR `IsAvailable Equals true` | 400 | 17 |
| C4 | `Name Contains Item` و`Filter.Groups[0]`: `IsAvailable Equals false` | 400 | 400 (كما قبل: المجموعات المتداخلة خارج النطاق) |
| C5 | `ProductPrices.UnitPrice GreaterThan 10` + `IsAvailable Equals true` | 400 | 7 |
| X1 | `Sort.SortBy=ProductPrices.UnitPrice` asc + `IsAvailable Equals true` | 400 | 14 مرتّبة بسعر القائمة (2, 3, 4 … 40) |
| X2 | `Sort.SortBy=StockStatusLabel` asc + `ProductPrices.UnitPrice GreaterThan 10` | 400 | 9، كل نص كتلة |
| X3 | `Sort.SortBy=ProductPrices.UnitPrice` desc + `StockStatusLabel In [JLoka-01, Back order]` | 400 | 8 مرتّبة بالسعر تنازلياً |
| S1/S2 | `Sort.SortBy=IsAvailable&Sort.Ascending=false/true` | 400 `Sorting by 'IsAvailable' is not supported.` | التوفّر ثم الكمية (القسم 4) |
| S3/S4 | `Sort.SortBy=StockStatusLabel&Sort.Ascending=true/false` | 400 | كل نص كتلة واحدة |
| S5 | `Sort.SortBy=stockstatuslabel` desc + `IsAvailable Equals true` | 400 | 14: JLoka-02 ← JLoka-01 ← Available brian 1 ← Available |
| S6 | S1 + `warehouseId=W1` | 400 | بكميات W1 |
| S7 | S1 + `pageSize=5&pageNumber=2` | 400 | العناصر 6..10، و`TotalCount` = 19 |
| E1 | `IsAvailable Equals yes` | 400 | 400 `Failed conversion from string to Boolean` = `AllowBackorder` |
| E2 / E12 | `IsAvailable Equals` / `NotEquals` بلا `Value` | 400 | 500 `Argument types do not match` = `AllowBackorder` |
| E3 | `IsAvailable GreaterThan true` | 400 | 400 `Invalid usage of Greater Than operator.` = `AllowBackorder` |
| E4 | `IsAvailable Contains t` | 400 | 400 `Failed conversion from string to Boolean` = `AllowBackorder` |
| E5 | `IsAvailable IsNull` | 400 | 400 `Operator IsNull is only supported for nullable fields.` = `AllowBackorder` |
| E6 | `IsAvailable In` بلا `Values` | 400 | 400 `In operator requires a list of values in 'Values' property.` = `AllowBackorder` |
| E7 | `StockStatusLabel GreaterThan A` | 400 | 500 كـ `Name GreaterThan` |
| E8 / E11 | `StockStatusLabel Contains` / `StartsWith` بلا `Value` | 400 | 0 منتج، كـ `Name` |
| E9 | `Filter.Logic=OR`: حقلنا + `Name` | 400 | 400 `Filtering by 'StockStatusLabel' is supported only with AND logic.` (قاعدتنا الوحيدة) |
| E10 | `StockStatusColor Equals …` | 400 | 400 (لم يُدعم، بلا تغيير) |

**الطلب نفسه بصيغة JSON** (هو ما يُبنى من الرابط بـ `[FromQuery]`)، مثال S5:

```json
{
  "pageNumber": 1, "pageSize": 200,
  "sort": { "sortBy": "StockStatusLabel", "ascending": false },
  "filter": { "logic": "AND", "conditions": [ { "field": "IsAvailable", "operator": "Equals", "value": "true" } ] }
}
```

**الصدى بعد المعالجة:**

```json
{ "filter": { "logic": "AND", "conditions": [], "groups": [] }, "totalCount": 14 }
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
  - **بدون** `OrderByDescending` أعاد الفلتر `StockStatusLabel Contains promo` **لا شيء**.
  - فالفلتر لم يطابق ما يُعرض في هذه الحالة النادرة.
- **التوصية:** يبقى معلّقاً كما طلبت حتى تجرّب. لكن تفعيله يعيد التطابق التام، وكلفته صفر تقريباً لأن الجدول صغير.

---

## 10. حدود معروفة ونقاط لا أغيّرها (للأمانة)

1. **`GetStockStatus` يقرأ أحدث 50 حالة فقط** (`PageSize` الافتراضي). نحن نقرأ كل الحالات. لا فرق إلا إن تجاوز عدد الحالات 50.
2. **النصوص الافتراضية الثلاثة مكررة** من `GetDefaultStockLabel`، ومعها قاعدة `GetAvailability`/`GetStockStatus` (لا مفرّ، فهي C# لكل منتج). إن غيّرها المؤلف يجب تغييرها في `ApplyStockStatus`.
2.1 **المجموعات المتداخلة (`Filter.Groups`) لحقلينا** خارج النطاق، كما ينظر فلتر قائمة الأسعار للمستوى الأعلى فقط. يرفضها المؤلف بخطئه المعتاد (400).
2.2 **سلوك المؤلف الحدّي يبقى كما هو** لأن باني المؤلف يبني الحقلين: `NotIn` على bool، أو bool بلا قيمة، يعطي 500 (كـ `AllowBackorder`). و`GreaterThan` على نص يعطي 500 (كـ `Name`). و`NotIn` على نص يعطي 400 `not implemented` (كـ `Name`).
2.3 **الاستعمال المباشر لـ `FilterBuilder.BuildPredicate` و`PropertyCache`:** هي عامة (`public`) في الحزمة، لكن لا خدمة أخرى في المشاريع تستدعيها مباشرة، فكل الخدمات تمرّ عبر `GenerateQuery`. لم نستعمل `GenerateQuery` هنا لأنه يعدّ ويرتّب ويقسّم الصفحات أيضاً، ونحن نحتاج منه جزء الفلتر فقط، وهو بالضبط ما يستدعيه `GenerateQuery` داخلياً. قد يسأل المراجِع عن هذا، وهذا جوابه.
3. **التداخل:** القسم 9.2.
4. **فلاتر العرض حسب الدور (`ApplyActionViewFilterAsync`)** تطبَّق على `CustomStockStatusService.Select` إن أُعدّت. استعلامنا يقرأ الجدول مباشرة كما يفعل `InitialDbSet` للخدمة.
5. **حساسية حالة الأحرف** في النص حسب الـ collation، كباقي فلاتر النص عند المؤلف.
6. **نقاط في كود المؤلف رأيتها ولم أمسّها:**
   - فلتر مدى السعر (شرطان) يصبح `Any` منفصلاً عن شرط قائمة الأسعار.
   - شرط قائمة الأسعار يُضمّ بـ OR تحت `Filter.Logic=OR`.
   - `TotalCount` في الاستجابة هو عدد عناصر الصفحة.
   - الحلقة تكلّف 4 رحلات أو أكثر إلى القاعدة لكل منتج، منها `CustomStockStatusService.Select` لكل منتج.
   - `new Guid(warehouseId)` يعطي 500 لقيمة غير صالحة.
7. **النشر:** التعديل داخل Products API فقط. حزمة Products المنشورة لا تحتاج إصداراً جديداً إلا إن أردت أن يرى Commerce/POD الكود نفسه. لا تغيير في عقد `ProductDto`.

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

1. نسخة احتياطية في `_backups\IC-2132-ApplyStockStatus-BEFORE-<الوقت>`، فيها `ProductService.cs` الحالي (`4081805d`)، و`restore.ps1` الذي يحذف الملف الجديد أيضاً.
2. إنشاء `Helper/ProductStockStatus.cs` (‏BOM + CRLF) بالنص المُختبَر حرفياً.
3. تعديل `ProductService.cs` (‏BOM + CRLF محفوظان):
   - `using ITM.Core.Organization.Helpers.Quering;`.
   - سطرا الاستدعاء قبل ترتيب السعر.
   - حذف مقطع التجربة (228–236).
   - إضافة `ApplyStockStatus`.
4. تحقّق Roslyn:
   - لم يتغيّر إلا `GetProductsForTransaction` مع الدالة الجديدة والـ `using`.
   - فلتر قائمة الأسعار وترتيب السعر بقيا حرفياً.
   - النص مطابق لما نجح في الاختبار 76/76.
   - `GetStockStatus` و`GetAvailability` و`GetDefaultStockLabel` لم تُمسّ.
5. عرض الـ diff والبصمات (MD5). البناء والتجربة عندك.
