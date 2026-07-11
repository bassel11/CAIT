# استبدال بيانات محافظة حلب ببيانات محافظة دمشق — تقرير كامل وخطوة بخطوة

**التاريخ:** 2026-07-11
**الحالة:** منفّذ ومُتحقَّق منه **محليًا فقط** (بيئة تطوير على Docker Desktop) — **لم يُنفَّذ أي `git commit` أو `push` بعد**.
**من نفّذه:** بمساعدة Claude Code، بالتنسيق المباشر مع صاحب المستودع.

---

## 1) السياق والهدف

النظام (**TRRCMS** — Tenure Rights Registration & Claims Management System، بُني لصالح UN-Habitat) كان يدعم **محافظة حلب فقط** كتقسيم إداري مزروع فعليًا في قاعدة البيانات. طُلب استبدال بيانات حلب بالكامل ببيانات **محافظة دمشق**، اعتمادًا على شيفايلات GIS رسمية (UN-OCHA) زُوِّدت في مجلد `syr_admin_shp_utf8_19321/`.

**قيد أساسي وُضع من البداية:** المشروع كان يُعتقد في البداية أنه بيئة Live/إنتاج — تبيّن لاحقًا أنه **نسخة محلية حديثة العهد على جهاز المطوّر (Windows 11 + Docker Desktop)**، مستنسخة من GitLab، **بدون أي بيانات تشغيلية حقيقية** (لا حالات، لا مسوحات، لا مبانٍ فعلية) — وهذا ما سمح بحذف بيانات حلب مباشرة بدل الاكتفاء بتعطيلها.

---

## 2) كيف كانت بيانات حلب مزروعة (قبل التغيير) — 3 مصادر منفصلة

| # | الآلية | الملف المصدر |
|---|---|---|
| 1 | تسلسل هرمي (محافظة→منطقة→ناحية→تجمع)، يُزرع فقط إذا كان جدول Governorates فارغًا عند إقلاع التطبيق | `src/TRRCMS.WebAPI/Data/administrative_divisions.json` عبر `AdministrativeHierarchySeeder.cs` |
| 2 | 109 حي في مدينة حلب (هندسة/حدود GIS)، تُزرع عبر EF Migration مخصّصة | `src/TRRCMS.Infrastructure/Data/aleppo_neighborhoods_v1.json` عبر migration `SeedAleppoNeighborhoodsFromGIS` |
| 3 | 87 مبنى تجريبي (نقاط GIS) في حلب، تُزرع عبر EF Migration | `src/TRRCMS.Infrastructure/Data/buildings_sample_v1.json` عبر migration `SeedSampleBuildingsFromGIS` |

اكتُشف أيضًا أن Migration سابقة (`AddCommunityExternalPCodeAndRealignAleppoCodes`) كانت قد نقلت كود حلب تاريخيًا من `01` إلى `02` (لمطابقة معيار OCHA) — وهذا بالضبط ما جعل كود `01` **متاحًا** لدمشق لاحقًا (لأن `SY01` هو كود دمشق الرسمي عند OCHA).

---

## 3) تحليل بيانات دمشق من `syr_admin_shp_utf8_19321/`

فُحصت كل الشيفايلات (`syr_admin0/1/2/3.shp`, `city_neighbourhoods.shp`, `syr_pplp_adm4_unocha.shp`) عبر مكتبة Python (`pyshp`)، **قراءة فقط دون أي تعديل**. تم استخراج سجلات **دمشق فقط** (وليس كل محافظات سوريا الـ14 الموجودة في نفس الملفات — خارج نطاق الطلب):

| المستوى | العدد المستخرَج | التفاصيل |
|---|---|---|
| المحافظة | 1 | دمشق، `PCODE=SY01` → **الكود الرقمي `01`** (متاح فعليًا) |
| المنطقة | 1 | دمشق، `SY0100` → كود `00` |
| الناحية | 1 | دمشق، `SY010000` → كود `00` |
| التجمعات | **2** | `C1001` دمشق (المركز) + `C1002` اليرموك — بخلاف حلب (تجمع واحد فقط) |
| الأحياء | **100** | 97 تحت تجمع دمشق + 3 تحت اليرموك، هندسة `PolygonZ` (Z=0 دائمًا) → توافق مباشر مع أسلوب WKT ثنائي الأبعاد المستخدم أصلًا لحلب |

جميع الهندسات (100/100) تحققت صحتها لاحقًا داخل PostGIS عبر `ST_IsValid()`.

---

## 4) قرارات حاسمة اتُّخذت أثناء التنفيذ (بالتنسيق مع صاحب المستودع)

1. **البيئة محلية وليست إنتاجًا** → سمح بحذف بيانات حلب فعليًا بدل تعطيلها فقط.
2. **الـ87 مبنى التجريبي المرتبط بحلب (`buildings_sample_v1.json`) يُحذف أيضًا** — قرار صريح، بدل تركه بيانات "يتيمة" بلا محافظة مرجعية بعد حذف حلب. (لا يوجد ملف مبانٍ مقابل لدمشق حاليًا).
3. **لا تُعدَّل أي Migration قديمة مطبَّقة سابقًا** (أفضل ممارسة EF Core) — كل تغيير عبر Migrations **جديدة** تُضاف فوق التاريخ الحالي فقط.
4. **لا يُعاد تسمية أي صنف برمجي قديم يحمل اسم "Aleppo"** (`AleppoNeighborhoodsImporter`, إلخ) — إعادة استخدام حرفية للمنطق البرمجي القائم (هو محايد تجاه المحافظة أصلًا) لتقليل نطاق التغيير والمخاطر، بدل إعادة تسمية شاملة غير ضرورية.

---

## 5) التنفيذ الفعلي — خطوة بخطوة بالترتيب الزمني

### أ) توليد بيانات دمشق من الشيفايلات
- سكربت Python (خارج المشروع، في مجلد عمل مؤقت) يستخدم `pyshp` لاستخراج سجلات دمشق وتحويلها لصيغتي JSON اللتين يفهمهما النظام أصلًا.
- الناتج: `damascus_administrative_divisions.json` (تسلسل هرمي) + `damascus_neighborhoods_v1.json` (100 حي بحدود WKT + مساحة تقريبية محسوبة).

### ب) تعديلات كود بسيطة (إعادة استخدام لا إعادة كتابة)
| الملف | التغيير |
|---|---|
| `src/TRRCMS.Infrastructure/TRRCMS.Infrastructure.csproj` | تسجيل `damascus_neighborhoods_v1.json` كمورد مضمّن (Embedded Resource) |
| `src/TRRCMS.Infrastructure/Persistence/SeedData/AleppoNeighborhoodsImporter.cs` | إضافة overload بسيط `LoadEmbedded(string resourceName)` (+16 سطر فقط) يسمح بتحميل أي مجموعة بيانات بنفس الصيغة — الأصناف/المنطق الأساسي لم يتغيّر |
| `src/TRRCMS.WebAPI/Data/administrative_divisions.json` | استُبدل محتواه بالكامل: حلب → دمشق |

### ج) 3 Migrations جديدة (بأسماء وترتيب زمني `20260711...`)
كل migration أُنشئت عبر الأمر الرسمي `dotnet ef migrations add <Name>` (وليس كتابة يدوية من الصفر) ثم عُبِّئ محتوى `Up()`/`Down()` يدويًا بـ SQL خام — **بنفس الأسلوب المستخدم أصلًا في migrations حلب** (`SeedAleppoNeighborhoodsFromGIS`, `AddCommunityExternalPCodeAndRealignAleppoCodes`، اللتين لا تحتويان أي تغيير Schema، فقط `migrationBuilder.Sql(...)`).

1. **`AddDamascusAdministrativeHierarchy`** — تُدرج: محافظة دمشق (01) + منطقة (00) + ناحية (00) + تجمعان (001 دمشق / 002 اليرموك).
2. **`SeedDamascusNeighborhoodsFromGIS`** — تُعيد استخدام `AleppoNeighborhoodsImporter.LoadEmbedded(...)` + `BuildSeedSqlStatements(...)` الموجودتين أصلًا، لتحميل `damascus_neighborhoods_v1.json` وإدراج الـ100 حي.
3. **`RemoveAleppoAdministrativeDataAndSampleBuildings`** — تحذف بترتيب الأوراق إلى الجذر (لاحترام قيود Foreign Key الحالية):
   `Buildings` → `StagingBuildings` → `Neighborhoods` → `Communities` → `SubDistricts` → `Districts` → `Governorates`، كل ذلك حيث `GovernorateCode`/`Code` = `'02'` (حلب).
   - `Down()` لهذه الـmigration **مقصود أن تكون غير قابلة للتراجع** (ترمي `NotSupportedException` بتوثيق واضح) لأنها تحذف بيانات مرجعية/seed لا Schema — التراجع الحقيقي الوحيد هو إعادة بناء القاعدة من الصفر (`docker compose down -v && up --build`) لإعادة تشغيل التاريخ الكامل لـMigrations.

### د) تشغيل البيئة والتحقق
- محاولة `docker compose up --build` (بناء صورة API كاملة) **فشلت مرتين متتاليتين** بسبب انقطاع شبكي متكرر أثناء تحميل حزمة NuGet واحدة (`QuestPDF`) عبر شبكة Docker الافتراضية على Windows — مشكلة بيئة/شبكة معروفة، **غير متعلقة بكودنا**.
- **الحل المعتمد (هجين)**: تشغيل **قاعدة البيانات فقط** عبر Docker (`docker compose up -d db adminer` — صورة `postgis/postgis:16-3.4-alpine` كانت محمَّلة مسبقًا بالكامل، فلا حاجة لشبكة)، وتشغيل الـ **API مباشرة على الجهاز** عبر `dotnet run` (الذي سبق أن نجح في استعادة نفس الحزمة عبر شبكة الجهاز المباشرة).
- طُبِّقت **كل** الـMigrations (67 migration تاريخية + 3 الجديدة) بأمر واحد ناجح:
  ```
  dotnet ef database update --project TRRCMS.Infrastructure --startup-project TRRCMS.WebAPI
  ```

---

## 6) نتائج التحقق النهائي (مؤكَّدة فعليًا، ليست افتراضية)

### على مستوى قاعدة البيانات (استعلام SQL مباشر)
| الجدول | النتيجة |
|---|---|
| `Governorates` | سجل واحد فقط: `01` دمشق |
| `Districts` / `SubDistricts` | دمشق/دمشق (00/00) |
| `Communities` | 2: `001` دمشق (C1001) + `002` اليرموك (C1002) |
| `Neighborhoods` | 100 (97+3)، **كل الهندسات صالحة** (`ST_IsValid`=true للكل، صفر NULL) |
| `Buildings` | 0 (حُذفت عيّنة حلب) |
| أي أثر لكود `02` (حلب) في كل الجداول أعلاه | **صفر تمامًا** |

### على مستوى الـ API الفعلي (طلبات حقيقية بعد تسجيل دخول Admin)
- `GET /api/v1/administrative-divisions/governorates` → دمشق فقط.
- `GET /api/v1/administrative-divisions/communities?governorateCode=01` → التجمعان الصحيحان.
- `GET /api/v1/Neighborhoods?...` → الأحياء بإحداثيات وحدود WKT صحيحة.

---

## 7) ما لم يتغيّر بعد (متبقٍّ في الكود، عن قصد — ليس نقصًا سهوًا)

هذه نقاط تخص **الكود المصدري فقط**، وليس لها أي أثر على بيانات قاعدة البيانات الفعلية (التي أصبحت نظيفة 100٪ من حلب):

- ملفات migrations حلب **القديمة** (`SeedAleppoNeighborhoodsFromGIS.cs`, `AddCommunityExternalPCodeAndRealignAleppoCodes.cs`, `SeedSampleBuildingsFromGIS.cs`) — **يُمنع حذفها/تعديلها** كأفضل ممارسة (سجل تاريخي ثابت لـEF Core).
- `src/TRRCMS.Infrastructure/Data/aleppo_neighborhoods_v1.json` (109 حي حلب) ما زال موجودًا كملف مضمّن.
- أسماء أصناف برمجية ما زالت تحمل اسم "Aleppo" حرفيًا: `AleppoNeighborhoodsImporter`, `IAleppoNeighborhoodsImportService`, `AleppoNeighborhoodsImportService`, `ImportAleppoNeighborhoodsCommand(Handler)`.
- نص توثيق Swagger لنقطة النهاية `POST /api/v1/Neighborhoods/import-bulk` لا يزال يقول: *"Bulk-import Aleppo neighborhoods from a GIS-team payload"*.
- تعليقات توثيقية (XML doc comments) في `Survey.cs`, `User.cs`, `SubDistrict.cs`, `CreateOfficeSurveyCommand.cs` تذكر "Aleppo" كمثال فقط (غير وظيفية).

> إذا رغب الفريق لاحقًا بتنظيف هذه الأسماء/النصوص بالكامل (إعادة تسمية شاملة)، هذه مهمة منفصلة يمكن تنفيذها لاحقًا دون أي أثر على البيانات.

---

## 8) قائمة كاملة بالملفات المتأثرة (git status وقت كتابة هذا التقرير)

```
معدَّلة:
  src/TRRCMS.Infrastructure/Persistence/SeedData/AleppoNeighborhoodsImporter.cs
  src/TRRCMS.Infrastructure/TRRCMS.Infrastructure.csproj
  src/TRRCMS.WebAPI/Data/administrative_divisions.json

جديدة (غير متتبَّعة بعد):
  src/TRRCMS.Infrastructure/Data/damascus_neighborhoods_v1.json
  src/TRRCMS.Infrastructure/Migrations/20260711113829_AddDamascusAdministrativeHierarchy.cs (+ .Designer.cs)
  src/TRRCMS.Infrastructure/Migrations/20260711113959_SeedDamascusNeighborhoodsFromGIS.cs (+ .Designer.cs)
  src/TRRCMS.Infrastructure/Migrations/20260711114046_RemoveAleppoAdministrativeDataAndSampleBuildings.cs (+ .Designer.cs)
  docs/DAMASCUS_MIGRATION.md  (هذا الملف)

محلي فقط (مستثنى من Git تلقائيًا عبر .gitignore، لا يظهر في status):
  src/TRRCMS.WebAPI/appsettings.Development.json  (سلسلة اتصال محلية لقاعدة بيانات Docker)
```

**لم يُنفَّذ `git add`/`git commit`/`git push` على أي من هذه الملفات حتى كتابة هذا التقرير.**

`ApplicationDbContextModelSnapshot.cs` **لم يتغيّر إطلاقًا** (diff فارغ) — تأكيد أن الـ3 Migrations الجديدة بيانات فقط، بدون أي تعديل على هيكل الجداول.

---

## 9) تعليمات التشغيل المحلي (للرجوع إليها لاحقًا)

```powershell
# تشغيل قاعدة البيانات فقط (بدون بناء API عبر Docker):
cd C:\Users\VICTUS\Desktop\HLP\hlp-backend
docker compose up -d db adminer

# تشغيل الـ API مباشرة (نافذة منفصلة):
cd C:\Users\VICTUS\Desktop\HLP\hlp-backend\src\TRRCMS.WebAPI
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --urls "http://localhost:5031"

# الوصول:
#   Swagger:  http://localhost:5031/swagger
#   Adminer:  http://localhost:8081  (Server: db, User: postgres, Password: ChangeThisPassword, DB: TRRCMS_Dev)

# الإيقاف (يحافظ على البيانات):
docker compose stop db adminer
# (أوقف نافذة dotnet run بـ Ctrl+C)
```

> ملاحظة: التطبيق يُطبّق Migrations تلقائيًا عند كل إقلاع (`context.Database.MigrateAsync()` في `WebApplicationExtensions.cs`)، سواء شُغِّل عبر Visual Studio أو `dotnet run` أو Docker — لا حاجة لأمر `dotnet ef database update` يدوي في كل مرة بعد التطبيق الأول.

---

## 10) الخطوات التالية الممكنة (بانتظار القرار)

- [ ] مراجعة `git diff` النهائي ثم `git commit` (محليًا) — **لم يُنفَّذ بعد**.
- [ ] اختياري: تنظيف تسميات "Aleppo" المتبقية في الكود (أصناف/نصوص توثيق) دون أثر على البيانات.
- [ ] اختياري: توفير بيانات مبانٍ عيّنة لدمشق (مقابلة لملف `buildings_sample_v1.json` القديم) إذا احتاج الفريق بيانات تجريبية على مبانٍ.
- [ ] عند الجاهزية: `git push` إلى الفرع المناسب وفتح Pull Request للمراجعة.
