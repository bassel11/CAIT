# probe33: the real model, the real GenerateQuery

## The data (all warehouses / W1), as GetStockStatus shows it

| product | AllowBackorder | stock rows (WH: OnHand - Committed) | available (all) | isAvailable | stockStatusLabel | available (W1) | label (W1) |
|---|---|---|---|---|---|---|---|
| Jafar-Product-03 | True | W1: 1000 - 0 | 1000.00 | True | Available | 1000.00 | Available |
| Loka-01 | True | W1: 200 - 0 | 200.00 | True | JLoka-02 | 200.00 | JLoka-02 |
| Test-MetaField | False | W1: 80 - 0 | 80.00 | True | JLoka-01 | 80.00 | JLoka-01 |
| Plug | True | W1: 50 - 0 | 50.00 | True | JLoka-01 | 50.00 | JLoka-01 |
| Alex Vando Shirt Red | True | W1: 30 - 0 | 30.00 | True | JLoka-01 | 30.00 | JLoka-01 |
| Alex Vando Shirt | True | W1: 12 - 0 | 12.00 | True | JLoka-01 | 12.00 | JLoka-01 |
| matt test child 2 | True | W1: 7 - 0 | 7.00 | True | Available | 7.00 | Available |
| iphone caver | True | W1: 5 - 0 | 5.00 | True | Available brian 1 | 5.00 | Available brian 1 |
| Nitrile Gloves Large | False | W1: 3 - 0 | 3.00 | True | Available brian 1 | 3.00 | Available brian 1 |
| Paintbrush | False | W1: 2 - 0 | 2.00 | True | Available brian 1 | 2.00 | Available brian 1 |
| Scrub Suit | True | W1: 1 - 0 | 1.00 | True | Available brian 1 | 1.00 | Available brian 1 |
| Multi Warehouse Item | False | W1: 3 - 0, W2: 60 - 0 | 63.00 | True | JLoka-01 | 3.00 | Available brian 1 |
| OnHand 4.5 Item | False | W1: 4.5 - 0 | 4.00 | True | Available brian 1 | 4.00 | Available brian 1 |
| Half Unit Item | False | W1: 1 - 0.5 | 0.50 | True | Available | 0.50 | Available |
| Committed Exceeds Item | True | W1: 5 - 8 | -3.00 | False | Back order | -3.00 | Back order |
| Back Order No Rows | True |  | 0.00 | False | Back order | 0.00 | Back order |
| Out Of Stock No Rows | False |  | 0.00 | False | Out of Stock | 0.00 | Out of Stock |
| Oversold Item | False | W1: 2 - 5 | -3.00 | False | Out of Stock | -3.00 | Out of Stock |
| Zero Stock Backorder | True | W1: 0 - 0 | 0.00 | False | Back order | 0.00 | Back order |
| Inactive Product (not listed) | False | W1: 50 - 0 | 50.00 | True | JLoka-01 | 50.00 | JLoka-01 |
| Hidden Product (not listed) | False | W1: 50 - 0 | 50.00 | True | JLoka-01 | 50.00 | JLoka-01 |
| Purchasing Only (not listed) | False | W1: 50 - 0 | 50.00 | True | JLoka-01 | 50.00 | JLoka-01 |

## 1. The author's requests: same SQL text, same products, same echo

- PASS default (no sort, no filter): same SQL text (19 products, TotalCount 19), same products in the same order, same echo
- PASS page 2 of 5: same SQL text (5 products, TotalCount 19), same products in the same order, same echo
- PASS Sort Name asc: same SQL text (19 products, TotalCount 19), same products in the same order, same echo
- PASS Sort CreateDate asc: same SQL text (19 products, TotalCount 19), same products in the same order, same echo
- PASS Sort ProductPrices.UnitPrice asc (the price list): same SQL text (19 products, TotalCount 19), same products in the same order, same echo
- PASS Sort productprices.unitprice desc: same SQL text (19 products, TotalCount 19), same products in the same order, same echo
- PASS Filter ProductPrices.UnitPrice GreaterThan 10 (+ the price list condition): same SQL text (9 products, TotalCount 9), same products in the same order, same echo
- PASS Filter ProductPrices.UnitPrice GreaterThan 10 + LessThan 25 (the author's two separate Any: noted): same SQL text (7 products, TotalCount 7), same products in the same order, same echo
- PASS Filter AllowBackorder Equals true: same SQL text (11 products, TotalCount 11), same products in the same order, same echo
- PASS Filter Name Contains Alex, Sort Name desc: same SQL text (2 products, TotalCount 2), same products in the same order, same echo
- PASS Search Item: same SQL text (5 products, TotalCount 5), same products in the same order, same echo
- PASS Filter.Logic OR: Name Contains Plug OR Name Contains Paint: same SQL text (2 products, TotalCount 2), same products in the same order, same echo
- PASS warehouse W1, default: same SQL text (19 products, TotalCount 19), same products in the same order, same echo
- PASS categoryId (none in that category): same SQL text (0 products, TotalCount 0), same products in the same order, same echo

## 2. The restored file (trial snippet, lines 228-236) against the author's code

- default: author `Zero Stock Backorder, Oversold Item, Out Of Stock No Rows, Back Order No Rows, Committed Exceeds Item, Half Unit Item`, … | restored file `Committed Exceeds Item, Oversold Item, Back Order No Rows, Out Of Stock No Rows, Zero Stock Backorder, Half Unit Item`, … | echo Sort author `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"TotalCount":19}` restored `{"TotalCount":19}` | same order: False
- Sort Name asc: author `Alex Vando Shirt, Alex Vando Shirt Red, Back Order No Rows, Committed Exceeds Item, Half Unit Item, iphone caver`, … | restored file `Committed Exceeds Item, Oversold Item, Back Order No Rows, Out Of Stock No Rows, Zero Stock Backorder, Half Unit Item`, … | echo Sort author `{"Sort":{"SortBy":"Name","Ascending":true,"IsMetaField":false},"TotalCount":19}` restored `{"TotalCount":19}` | same order: False
- Sort ProductPrices.UnitPrice asc: author `Oversold Item, Half Unit Item, Plug, Nitrile Gloves Large, OnHand 4.5 Item, Paintbrush`, … | restored file `Committed Exceeds Item, Oversold Item, Back Order No Rows, Out Of Stock No Rows, Zero Stock Backorder, Half Unit Item`, … | echo Sort author `{"TotalCount":19}` restored `{"TotalCount":19}` | same order: False

ORDER BY of the restored file:
```sql
ORDER BY (
        SELECT TOP(1) [c].[Id]
        FROM [CustomStockStatuses] AS [c]
        WHERE CAST([c].[QuantityFrom] AS DECIMAL(18,2)) <= (
            SELECT COALESCE(SUM([p0].[OnHand] - [p0].[Committed]), 0.0)
            FROM [ProductAvailabilities] AS [p0]
            WHERE [p].[Id] = [p0].[ProductId]) AND CAST([c].[QuantityTo] AS DECIMAL(18,2)) >= (
            SELECT COALESCE(SUM([p1].[OnHand] - [p1].[Committed]), 0.0)
            FROM [ProductAvailabilities] AS [p1]
            WHERE [p].[Id] = [p1].[ProductId])), (
        SELECT COALESCE(SUM([p2].[OnHand] - [p2].[Committed]), 0.0)
        FROM [ProductAvailabilities] AS [p2]
        WHERE [p].[Id] = [p2].[ProductId])
    OFFSET @__p_0 ROWS FETCH NEXT @__p_1 ROWS ONLY
) AS [t]
LEFT JOIN [TaxClasses] AS [t0] ON [t].[TaxClassId] = [t0].[Id]
LEFT JOIN [UnitOfMeasurements] AS [u] ON [t].[InventoryUnitOfMeasurementId] = [u].[Id]
ORDER BY [t].[c], [t].[c0]
```

## 3. Stock filters and sorts: before / after

### F1. IsAvailable Equals true

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=IsAvailable&Filter.Conditions[0].Operator=Equals&Filter.Conditions[0].Value=true`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"Equals","Value":"true","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): TotalCount 14, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":14}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Half Unit Item | True | Available | 0.50 | False |
| 2 | OnHand 4.5 Item | True | Available brian 1 | 4.00 | False |
| 3 | Multi Warehouse Item | True | JLoka-01 | 63.00 | False |
| 4 | Scrub Suit | True | Available brian 1 | 1.00 | True |
| 5 | Paintbrush | True | Available brian 1 | 2.00 | False |
| 6 | Nitrile Gloves Large | True | Available brian 1 | 3.00 | False |
| 7 | iphone caver | True | Available brian 1 | 5.00 | True |
| 8 | matt test child 2 | True | Available | 7.00 | True |
| 9 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 10 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 11 | Plug | True | JLoka-01 | 50.00 | True |
| 12 | Test-MetaField | True | JLoka-01 | 80.00 | False |
| 13 | Loka-01 | True | JLoka-02 | 200.00 | True |
| 14 | Jafar-Product-03 | True | Available | 1000.00 | True |

- PASS F1 IsAvailable Equals true: 14 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 14
<details><summary>SQL</summary>

```sql
DECLARE @__p_0 int = 0;
DECLARE @__p_1 int = 200;

SELECT [t].[Id], [t].[AllowBackorder], [t].[BOMType], [t].[Barcode], [t].[BrandId], [t].[CountryOfOriginId], [t].[CreateDate], [t].[CreatedBy], [t].[Description], [t].[DisplayVariantsAsList], [t].[Height], [t].[HeightUnitId], [t].[InventoryItem], [t].[InventoryUnitOfMeasurementId], [t].[IsSubscribed], [t].[Length], [t].[LengthUnitId], [t].[ManufacturerId], [t].[MaximumOrderQuantity], [t].[MetaFieldSetId], [t].[MinimumOrderQuantity], [t].[Name], [t].[ProductClassId], [t].[ProductState], [t].[ProductType], [t].[ProductVisibility], [t].[PurchasingItem], [t].[QuantityMultiplier], [t].[Remarks], [t].[SKU], [t].[SalesItem], [t].[SeriesId], [t].[SeriesNumber], [t].[ShipTypeId], [t].[ShortDescription], [t].[TaxClassId], [t].[UnitOfMeasurementGroupId], [t].[UpdateDate], [t].[UpdatedBy], [t].[Version], [t].[WarrantyTemplateId], [t].[Weight], [t].[WeightUnitId], [t].[Width], [t].[WidthUnitId], [t0].[Id], [t0].[CreateDate], [t0].[CreatedBy], [t0].[Description], [t0].[Name], [t0].[Rate], [t0].[UpdateDate], [t0].[UpdatedBy], [t0].[Version], [u].[Id], [u].[Active], [u].[Code], [u].[CreateDate], [u].[CreatedBy], [u].[Height], [u].[HeightUnitId], [u].[Length], [u].[LengthUnitId], [u].[Name], [u].[UpdateDate], [u].[UpdatedBy], [u].[Version], [u].[Weight], [u].[WeightUnitId], [u].[Width], [u].[WidthUnitId]
FROM (
    SELECT [p].[Id], [p].[AllowBackorder], [p].[BOMType], [p].[Barcode], [p].[BrandId], [p].[CountryOfOriginId], [p].[CreateDate], [p].[CreatedBy], [p].[Description], [p].[DisplayVariantsAsList], [p].[Height], [p].[HeightUnitId], [p].[InventoryItem], [p].[InventoryUnitOfMeasurementId], [p].[IsSubscribed], [p].[Length], [p].[LengthUnitId], [p].[ManufacturerId], [p].[MaximumOrderQuantity], [p].[MetaFieldSetId], [p].[MinimumOrderQuantity], [p].[Name], [p].[ProductClassId], [p].[ProductState], [p].[ProductType], [p].[ProductVisibility], [p].[PurchasingItem], [p].[QuantityMultiplier], [p].[Remarks], [p].[SKU], [p].[SalesItem], [p].[SeriesId], [p].[SeriesNumber], [p].[ShipTypeId], [p].[ShortDescription], [p].[TaxClassId], [p].[UnitOfMeasurementGroupId], [p].[UpdateDate], [p].[UpdatedBy], [p].[Version], [p].[WarrantyTemplateId], [p].[Weight], [p].[WeightUnitId], [p].[Width], [p].[WidthUnitId]
    FROM [Products] AS [p]
    WHERE [p].[SalesItem] = CAST(1 AS bit) AND [p].[ProductState] = 1 AND [p].[ProductVisibility] <> 20 AND COALESCE((
        SELECT COALESCE(SUM(CAST(CAST([p0].[OnHand] AS bigint) AS DECIMAL(18,2)) - [p0].[Committed]), 0.0)
        FROM [ProductAvailabilities] AS [p0]
        WHERE [p].[Id] = [p0].[ProductId]), 0.0) > 0.0
    ORDER BY [p].[CreateDate] DESC
    OFFSET @__p_0 ROWS FETCH NEXT @__p_1 ROWS ONLY
) AS [t]
LEFT JOIN [TaxClasses] AS [t0] ON [t].[TaxClassId] = [t0].[Id]
LEFT JOIN [UnitOfMeasurements] AS [u] ON [t].[InventoryUnitOfMeasurementId] = [u].[Id]
ORDER BY [t].[CreateDate] DESC
```
</details>


### F2. IsAvailable Equals false

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=IsAvailable&Filter.Conditions[0].Operator=Equals&Filter.Conditions[0].Value=false`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"Equals","Value":"false","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): TotalCount 5, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":5}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Zero Stock Backorder | False | Back order | 0.00 | True |
| 2 | Oversold Item | False | Out of Stock | -3.00 | False |
| 3 | Out Of Stock No Rows | False | Out of Stock | 0.00 | False |
| 4 | Back Order No Rows | False | Back order | 0.00 | True |
| 5 | Committed Exceeds Item | False | Back order | -3.00 | True |

- PASS F2 IsAvailable Equals false: 5 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 5

### F3. isavailable Equals True (any case)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=isavailable&Filter.Conditions[0].Operator=Equals&Filter.Conditions[0].Value=True`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"isavailable","Operator":"Equals","Value":"True","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'isavailable' is not allowed.`
- after (the proposal): TotalCount 14, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":14}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Half Unit Item | True | Available | 0.50 | False |
| 2 | OnHand 4.5 Item | True | Available brian 1 | 4.00 | False |
| 3 | Multi Warehouse Item | True | JLoka-01 | 63.00 | False |
| 4 | Scrub Suit | True | Available brian 1 | 1.00 | True |
| 5 | Paintbrush | True | Available brian 1 | 2.00 | False |
| 6 | Nitrile Gloves Large | True | Available brian 1 | 3.00 | False |
| 7 | iphone caver | True | Available brian 1 | 5.00 | True |
| 8 | matt test child 2 | True | Available | 7.00 | True |
| 9 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 10 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 11 | Plug | True | JLoka-01 | 50.00 | True |
| 12 | Test-MetaField | True | JLoka-01 | 80.00 | False |
| 13 | Loka-01 | True | JLoka-02 | 200.00 | True |
| 14 | Jafar-Product-03 | True | Available | 1000.00 | True |

- PASS F3 isavailable Equals True (any case): 14 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 14

### F4. IsAvailable NotEquals true

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=IsAvailable&Filter.Conditions[0].Operator=NotEquals&Filter.Conditions[0].Value=true`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"NotEquals","Value":"true","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): TotalCount 5, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":5}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Zero Stock Backorder | False | Back order | 0.00 | True |
| 2 | Oversold Item | False | Out of Stock | -3.00 | False |
| 3 | Out Of Stock No Rows | False | Out of Stock | 0.00 | False |
| 4 | Back Order No Rows | False | Back order | 0.00 | True |
| 5 | Committed Exceeds Item | False | Back order | -3.00 | True |

- PASS F4 IsAvailable NotEquals true: 5 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 5

### F5. IsAvailable In [true, false]

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=IsAvailable&Filter.Conditions[0].Operator=In&Filter.Conditions[0].Values[0]=true&Filter.Conditions[0].Values[1]=false`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"In","Values":["true","false"],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): TotalCount 19, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":19}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Zero Stock Backorder | False | Back order | 0.00 | True |
| 2 | Oversold Item | False | Out of Stock | -3.00 | False |
| 3 | Out Of Stock No Rows | False | Out of Stock | 0.00 | False |
| 4 | Back Order No Rows | False | Back order | 0.00 | True |
| 5 | Committed Exceeds Item | False | Back order | -3.00 | True |
| 6 | Half Unit Item | True | Available | 0.50 | False |
| 7 | OnHand 4.5 Item | True | Available brian 1 | 4.00 | False |
| 8 | Multi Warehouse Item | True | JLoka-01 | 63.00 | False |
| 9 | Scrub Suit | True | Available brian 1 | 1.00 | True |
| 10 | Paintbrush | True | Available brian 1 | 2.00 | False |
| 11 | Nitrile Gloves Large | True | Available brian 1 | 3.00 | False |
| 12 | iphone caver | True | Available brian 1 | 5.00 | True |
| 13 | matt test child 2 | True | Available | 7.00 | True |
| 14 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 15 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 16 | Plug | True | JLoka-01 | 50.00 | True |
| 17 | Test-MetaField | True | JLoka-01 | 80.00 | False |
| 18 | Loka-01 | True | JLoka-02 | 200.00 | True |
| 19 | Jafar-Product-03 | True | Available | 1000.00 | True |

- PASS F5 IsAvailable In [true, false]: 19 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 19

### F6. IsAvailable NotIn [true]

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=IsAvailable&Filter.Conditions[0].Operator=NotIn&Filter.Conditions[0].Values[0]=true`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"NotIn","Values":["true"],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): TotalCount 5, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":5}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Zero Stock Backorder | False | Back order | 0.00 | True |
| 2 | Oversold Item | False | Out of Stock | -3.00 | False |
| 3 | Out Of Stock No Rows | False | Out of Stock | 0.00 | False |
| 4 | Back Order No Rows | False | Back order | 0.00 | True |
| 5 | Committed Exceeds Item | False | Back order | -3.00 | True |

- PASS F6 IsAvailable NotIn [true]: 5 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 5

### F7. IsAvailable Equals true, warehouse W1

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=IsAvailable&Filter.Conditions[0].Operator=Equals&Filter.Conditions[0].Value=true&warehouseId=11111111-1111-1111-1111-111111111111`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"Equals","Value":"true","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): TotalCount 14, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":14}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Half Unit Item | True | Available | 0.50 | False |
| 2 | OnHand 4.5 Item | True | Available brian 1 | 4.00 | False |
| 3 | Multi Warehouse Item | True | Available brian 1 | 3.00 | False |
| 4 | Scrub Suit | True | Available brian 1 | 1.00 | True |
| 5 | Paintbrush | True | Available brian 1 | 2.00 | False |
| 6 | Nitrile Gloves Large | True | Available brian 1 | 3.00 | False |
| 7 | iphone caver | True | Available brian 1 | 5.00 | True |
| 8 | matt test child 2 | True | Available | 7.00 | True |
| 9 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 10 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 11 | Plug | True | JLoka-01 | 50.00 | True |
| 12 | Test-MetaField | True | JLoka-01 | 80.00 | False |
| 13 | Loka-01 | True | JLoka-02 | 200.00 | True |
| 14 | Jafar-Product-03 | True | Available | 1000.00 | True |

- PASS F7 IsAvailable Equals true, warehouse W1: 14 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 14

### F8. IsAvailable Equals false + AllowBackorder Equals true (back order)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=IsAvailable&Filter.Conditions[0].Operator=Equals&Filter.Conditions[0].Value=false&Filter.Conditions[1].Field=AllowBackorder&Filter.Conditions[1].Operator=Equals&Filter.Conditions[1].Value=true`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"Equals","Value":"false","Values":[],"IsMetaField":false},{"Field":"AllowBackorder","Operator":"Equals","Value":"true","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): TotalCount 3, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[{"Field":"AllowBackorder","Operator":"Equals","Value":"true","Values":[],"IsMetaField":false}],"Groups":[]},"TotalCount":3}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Zero Stock Backorder | False | Back order | 0.00 | True |
| 2 | Back Order No Rows | False | Back order | 0.00 | True |
| 3 | Committed Exceeds Item | False | Back order | -3.00 | True |

- PASS F8 IsAvailable Equals false + AllowBackorder Equals true (back order): 3 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 3

### F9. IsAvailable Equals true + IsAvailable Equals false (each condition narrows: AND, so none)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=IsAvailable&Filter.Conditions[0].Operator=Equals&Filter.Conditions[0].Value=true&Filter.Conditions[1].Field=IsAvailable&Filter.Conditions[1].Operator=Equals&Filter.Conditions[1].Value=false`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"Equals","Value":"true","Values":[],"IsMetaField":false},{"Field":"IsAvailable","Operator":"Equals","Value":"false","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): TotalCount 0, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":0}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|

- PASS F9 IsAvailable Equals true + IsAvailable Equals false (each condition narrows: AND, so none): 0 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 0

### L1. StockStatusLabel Equals JLoka-01

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=Equals&Filter.Conditions[0].Value=JLoka-01`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"Equals","Value":"JLoka-01","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): TotalCount 5, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":5}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Multi Warehouse Item | True | JLoka-01 | 63.00 | False |
| 2 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 3 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 4 | Plug | True | JLoka-01 | 50.00 | True |
| 5 | Test-MetaField | True | JLoka-01 | 80.00 | False |

- PASS L1 StockStatusLabel Equals JLoka-01: 5 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 5

### L2. StockStatusLabel Equals jloka-01 (collation, case-insensitive)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=Equals&Filter.Conditions[0].Value=jloka-01`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"Equals","Value":"jloka-01","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): TotalCount 5, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":5}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Multi Warehouse Item | True | JLoka-01 | 63.00 | False |
| 2 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 3 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 4 | Plug | True | JLoka-01 | 50.00 | True |
| 5 | Test-MetaField | True | JLoka-01 | 80.00 | False |

- PASS L2 StockStatusLabel Equals jloka-01 (collation, case-insensitive): 5 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 5

### L3. StockStatusLabel Equals Available (the default label only)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=Equals&Filter.Conditions[0].Value=Available`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"Equals","Value":"Available","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): TotalCount 3, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":3}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Half Unit Item | True | Available | 0.50 | False |
| 2 | matt test child 2 | True | Available | 7.00 | True |
| 3 | Jafar-Product-03 | True | Available | 1000.00 | True |

- PASS L3 StockStatusLabel Equals Available (the default label only): 3 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 3

### L4. StockStatusLabel NotEquals Available

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=NotEquals&Filter.Conditions[0].Value=Available`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"NotEquals","Value":"Available","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): TotalCount 16, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":16}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Zero Stock Backorder | False | Back order | 0.00 | True |
| 2 | Oversold Item | False | Out of Stock | -3.00 | False |
| 3 | Out Of Stock No Rows | False | Out of Stock | 0.00 | False |
| 4 | Back Order No Rows | False | Back order | 0.00 | True |
| 5 | Committed Exceeds Item | False | Back order | -3.00 | True |
| 6 | OnHand 4.5 Item | True | Available brian 1 | 4.00 | False |
| 7 | Multi Warehouse Item | True | JLoka-01 | 63.00 | False |
| 8 | Scrub Suit | True | Available brian 1 | 1.00 | True |
| 9 | Paintbrush | True | Available brian 1 | 2.00 | False |
| 10 | Nitrile Gloves Large | True | Available brian 1 | 3.00 | False |
| 11 | iphone caver | True | Available brian 1 | 5.00 | True |
| 12 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 13 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 14 | Plug | True | JLoka-01 | 50.00 | True |
| 15 | Test-MetaField | True | JLoka-01 | 80.00 | False |
| 16 | Loka-01 | True | JLoka-02 | 200.00 | True |

- PASS L4 StockStatusLabel NotEquals Available: 16 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 16

### L5. StockStatusLabel Contains JLoka

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=Contains&Filter.Conditions[0].Value=JLoka`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"Contains","Value":"JLoka","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): TotalCount 6, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":6}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Multi Warehouse Item | True | JLoka-01 | 63.00 | False |
| 2 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 3 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 4 | Plug | True | JLoka-01 | 50.00 | True |
| 5 | Test-MetaField | True | JLoka-01 | 80.00 | False |
| 6 | Loka-01 | True | JLoka-02 | 200.00 | True |

- PASS L5 StockStatusLabel Contains JLoka: 6 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 6
<details><summary>SQL</summary>

```sql
DECLARE @__text_0_contains nvarchar(100) = N'%JLoka%';
DECLARE @__p_1 int = 0;
DECLARE @__p_2 int = 200;

SELECT [t].[Id], [t].[AllowBackorder], [t].[BOMType], [t].[Barcode], [t].[BrandId], [t].[CountryOfOriginId], [t].[CreateDate], [t].[CreatedBy], [t].[Description], [t].[DisplayVariantsAsList], [t].[Height], [t].[HeightUnitId], [t].[InventoryItem], [t].[InventoryUnitOfMeasurementId], [t].[IsSubscribed], [t].[Length], [t].[LengthUnitId], [t].[ManufacturerId], [t].[MaximumOrderQuantity], [t].[MetaFieldSetId], [t].[MinimumOrderQuantity], [t].[Name], [t].[ProductClassId], [t].[ProductState], [t].[ProductType], [t].[ProductVisibility], [t].[PurchasingItem], [t].[QuantityMultiplier], [t].[Remarks], [t].[SKU], [t].[SalesItem], [t].[SeriesId], [t].[SeriesNumber], [t].[ShipTypeId], [t].[ShortDescription], [t].[TaxClassId], [t].[UnitOfMeasurementGroupId], [t].[UpdateDate], [t].[UpdatedBy], [t].[Version], [t].[WarrantyTemplateId], [t].[Weight], [t].[WeightUnitId], [t].[Width], [t].[WidthUnitId], [t0].[Id], [t0].[CreateDate], [t0].[CreatedBy], [t0].[Description], [t0].[Name], [t0].[Rate], [t0].[UpdateDate], [t0].[UpdatedBy], [t0].[Version], [u].[Id], [u].[Active], [u].[Code], [u].[CreateDate], [u].[CreatedBy], [u].[Height], [u].[HeightUnitId], [u].[Length], [u].[LengthUnitId], [u].[Name], [u].[UpdateDate], [u].[UpdatedBy], [u].[Version], [u].[Weight], [u].[WeightUnitId], [u].[Width], [u].[WidthUnitId]
FROM (
    SELECT [p].[Id], [p].[AllowBackorder], [p].[BOMType], [p].[Barcode], [p].[BrandId], [p].[CountryOfOriginId], [p].[CreateDate], [p].[CreatedBy], [p].[Description], [p].[DisplayVariantsAsList], [p].[Height], [p].[HeightUnitId], [p].[InventoryItem], [p].[InventoryUnitOfMeasurementId], [p].[IsSubscribed], [p].[Length], [p].[LengthUnitId], [p].[ManufacturerId], [p].[MaximumOrderQuantity], [p].[MetaFieldSetId], [p].[MinimumOrderQuantity], [p].[Name], [p].[ProductClassId], [p].[ProductState], [p].[ProductType], [p].[ProductVisibility], [p].[PurchasingItem], [p].[QuantityMultiplier], [p].[Remarks], [p].[SKU], [p].[SalesItem], [p].[SeriesId], [p].[SeriesNumber], [p].[ShipTypeId], [p].[ShortDescription], [p].[TaxClassId], [p].[UnitOfMeasurementGroupId], [p].[UpdateDate], [p].[UpdatedBy], [p].[Version], [p].[WarrantyTemplateId], [p].[Weight], [p].[WeightUnitId], [p].[Width], [p].[WidthUnitId]
    FROM [Products] AS [p]
    WHERE [p].[SalesItem] = CAST(1 AS bit) AND [p].[ProductState] = 1 AND [p].[ProductVisibility] <> 20 AND COALESCE((
        SELECT TOP(1) [c].[Label]
        FROM [CustomStockStatuses] AS [c]
        WHERE [c].[QuantityFrom] <= CAST(COALESCE((
            SELECT COALESCE(SUM(CAST(CAST([p0].[OnHand] AS bigint) AS DECIMAL(18,2)) - [p0].[Committed]), 0.0)
            FROM [ProductAvailabilities] AS [p0]
            WHERE [p].[Id] = [p0].[ProductId]), 0.0) AS int) AND [c].[QuantityTo] >= CAST(COALESCE((
            SELECT COALESCE(SUM(CAST(CAST([p1].[OnHand] AS bigint) AS DECIMAL(18,2)) - [p1].[Committed]), 0.0)
            FROM [ProductAvailabilities] AS [p1]
            WHERE [p].[Id] = [p1].[ProductId]), 0.0) AS int)), CASE
        WHEN COALESCE((
            SELECT COALESCE(SUM(CAST(CAST([p2].[OnHand] AS bigint) AS DECIMAL(18,2)) - [p2].[Committed]), 0.0)
            FROM [ProductAvailabilities] AS [p2]
            WHERE [p].[Id] = [p2].[ProductId]), 0.0) > 0.0 THEN N'Available'
        WHEN [p].[AllowBackorder] = CAST(1 AS bit) THEN N'Back order'
        ELSE N'Out of Stock'
    END) LIKE @__text_0_contains ESCAPE N'\'
    ORDER BY [p].[CreateDate] DESC
    OFFSET @__p_1 ROWS FETCH NEXT @__p_2 ROWS ONLY
) AS [t]
LEFT JOIN [TaxClasses] AS [t0] ON [t].[TaxClassId] = [t0].[Id]
LEFT JOIN [UnitOfMeasurements] AS [u] ON [t].[InventoryUnitOfMeasurementId] = [u].[Id]
ORDER BY [t].[CreateDate] DESC
```
</details>


### L6. StockStatusLabel Contains available (custom + default)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=Contains&Filter.Conditions[0].Value=available`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"Contains","Value":"available","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): TotalCount 8, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":8}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Half Unit Item | True | Available | 0.50 | False |
| 2 | OnHand 4.5 Item | True | Available brian 1 | 4.00 | False |
| 3 | Scrub Suit | True | Available brian 1 | 1.00 | True |
| 4 | Paintbrush | True | Available brian 1 | 2.00 | False |
| 5 | Nitrile Gloves Large | True | Available brian 1 | 3.00 | False |
| 6 | iphone caver | True | Available brian 1 | 5.00 | True |
| 7 | matt test child 2 | True | Available | 7.00 | True |
| 8 | Jafar-Product-03 | True | Available | 1000.00 | True |

- PASS L6 StockStatusLabel Contains available (custom + default): 8 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 8

### L7. StockStatusLabel Contains order

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=Contains&Filter.Conditions[0].Value=order`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"Contains","Value":"order","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): TotalCount 3, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":3}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Zero Stock Backorder | False | Back order | 0.00 | True |
| 2 | Back Order No Rows | False | Back order | 0.00 | True |
| 3 | Committed Exceeds Item | False | Back order | -3.00 | True |

- PASS L7 StockStatusLabel Contains order: 3 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 3

### L8. StockStatusLabel StartsWith Out

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=StartsWith&Filter.Conditions[0].Value=Out`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"StartsWith","Value":"Out","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): TotalCount 2, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":2}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Oversold Item | False | Out of Stock | -3.00 | False |
| 2 | Out Of Stock No Rows | False | Out of Stock | 0.00 | False |

- PASS L8 StockStatusLabel StartsWith Out: 2 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 2

### L9. StockStatusLabel EndsWith 01

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=EndsWith&Filter.Conditions[0].Value=01`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"EndsWith","Value":"01","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): TotalCount 5, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":5}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Multi Warehouse Item | True | JLoka-01 | 63.00 | False |
| 2 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 3 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 4 | Plug | True | JLoka-01 | 50.00 | True |
| 5 | Test-MetaField | True | JLoka-01 | 80.00 | False |

- PASS L9 StockStatusLabel EndsWith 01: 5 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 5

### L10. StockStatusLabel In [Back order, Out of Stock]

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=In&Filter.Conditions[0].Values[0]=Back%20order&Filter.Conditions[0].Values[1]=Out%20of%20Stock`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"In","Values":["Back order","Out of Stock"],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): TotalCount 5, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":5}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Zero Stock Backorder | False | Back order | 0.00 | True |
| 2 | Oversold Item | False | Out of Stock | -3.00 | False |
| 3 | Out Of Stock No Rows | False | Out of Stock | 0.00 | False |
| 4 | Back Order No Rows | False | Back order | 0.00 | True |
| 5 | Committed Exceeds Item | False | Back order | -3.00 | True |

- PASS L10 StockStatusLabel In [Back order, Out of Stock]: 5 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 5

### L11. StockStatusLabel NotIn [Available, Available brian 1]

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=NotIn&Filter.Conditions[0].Values[0]=Available&Filter.Conditions[0].Values[1]=Available%20brian%201`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"NotIn","Values":["Available","Available brian 1"],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): TotalCount 11, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":11}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Zero Stock Backorder | False | Back order | 0.00 | True |
| 2 | Oversold Item | False | Out of Stock | -3.00 | False |
| 3 | Out Of Stock No Rows | False | Out of Stock | 0.00 | False |
| 4 | Back Order No Rows | False | Back order | 0.00 | True |
| 5 | Committed Exceeds Item | False | Back order | -3.00 | True |
| 6 | Multi Warehouse Item | True | JLoka-01 | 63.00 | False |
| 7 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 8 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 9 | Plug | True | JLoka-01 | 50.00 | True |
| 10 | Test-MetaField | True | JLoka-01 | 80.00 | False |
| 11 | Loka-01 | True | JLoka-02 | 200.00 | True |

- PASS L11 StockStatusLabel NotIn [Available, Available brian 1]: 11 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 11

### L12. StockStatusLabel Equals JLoka-01 + Equals JLoka-02 (each condition narrows: AND, so none; send In for several values)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=Equals&Filter.Conditions[0].Value=JLoka-01&Filter.Conditions[1].Field=StockStatusLabel&Filter.Conditions[1].Operator=Equals&Filter.Conditions[1].Value=JLoka-02`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"Equals","Value":"JLoka-01","Values":[],"IsMetaField":false},{"Field":"StockStatusLabel","Operator":"Equals","Value":"JLoka-02","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): TotalCount 0, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":0}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|

- PASS L12 StockStatusLabel Equals JLoka-01 + Equals JLoka-02 (each condition narrows: AND, so none; send In for several values): 0 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 0

### L13. StockStatusLabel Contains JLoka + NotEquals JLoka-02 (two operators: AND)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=Contains&Filter.Conditions[0].Value=JLoka&Filter.Conditions[1].Field=StockStatusLabel&Filter.Conditions[1].Operator=NotEquals&Filter.Conditions[1].Value=JLoka-02`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"Contains","Value":"JLoka","Values":[],"IsMetaField":false},{"Field":"StockStatusLabel","Operator":"NotEquals","Value":"JLoka-02","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): TotalCount 5, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":5}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Multi Warehouse Item | True | JLoka-01 | 63.00 | False |
| 2 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 3 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 4 | Plug | True | JLoka-01 | 50.00 | True |
| 5 | Test-MetaField | True | JLoka-01 | 80.00 | False |

- PASS L13 StockStatusLabel Contains JLoka + NotEquals JLoka-02 (two operators: AND): 5 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 5

### L14. StockStatusLabel Contains % (a wildcard is a literal)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=Contains&Filter.Conditions[0].Value=%25`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"Contains","Value":"%","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): TotalCount 0, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":0}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|

- PASS L14 StockStatusLabel Contains % (a wildcard is a literal): 0 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 0

### L15. StockStatusLabel IsNull (not an operator of the proposal: a label is never null)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=IsNull`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"IsNull","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): `CustomInvalidDataException: StockStatusLabel can be filtered only with Equals, NotEquals, In, NotIn, Contains, StartsWith or EndsWith.`
- PASS L15 StockStatusLabel IsNull (not an operator of the proposal: a label is never null): rejected by the function with CustomInvalidDataException (400): CustomInvalidDataException: StockStatusLabel can be filtered only with Equals, NotEquals, In, NotIn, Contains, StartsWith or EndsWith.

### L16. StockStatusLabel NotNull

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=NotNull`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"NotNull","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): `CustomInvalidDataException: StockStatusLabel can be filtered only with Equals, NotEquals, In, NotIn, Contains, StartsWith or EndsWith.`
- PASS L16 StockStatusLabel NotNull: rejected by the function with CustomInvalidDataException (400): CustomInvalidDataException: StockStatusLabel can be filtered only with Equals, NotEquals, In, NotIn, Contains, StartsWith or EndsWith.

### L17. StockStatusLabel Equals Available brian 1, warehouse W1

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=Equals&Filter.Conditions[0].Value=Available%20brian%201&warehouseId=11111111-1111-1111-1111-111111111111`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"Equals","Value":"Available brian 1","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): TotalCount 6, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":6}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | OnHand 4.5 Item | True | Available brian 1 | 4.00 | False |
| 2 | Multi Warehouse Item | True | Available brian 1 | 3.00 | False |
| 3 | Scrub Suit | True | Available brian 1 | 1.00 | True |
| 4 | Paintbrush | True | Available brian 1 | 2.00 | False |
| 5 | Nitrile Gloves Large | True | Available brian 1 | 3.00 | False |
| 6 | iphone caver | True | Available brian 1 | 5.00 | True |

- PASS L17 StockStatusLabel Equals Available brian 1, warehouse W1: 6 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 6

### C1. StockStatusLabel Contains JLoka + Name Contains Alex

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=Contains&Filter.Conditions[0].Value=JLoka&Filter.Conditions[1].Field=Name&Filter.Conditions[1].Operator=Contains&Filter.Conditions[1].Value=Alex`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"Contains","Value":"JLoka","Values":[],"IsMetaField":false},{"Field":"Name","Operator":"Contains","Value":"Alex","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): TotalCount 2, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[{"Field":"Name","Operator":"Contains","Value":"Alex","Values":[],"IsMetaField":false}],"Groups":[]},"TotalCount":2}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 2 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |

- PASS C1 StockStatusLabel Contains JLoka + Name Contains Alex: 2 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 2

### C2. IsAvailable Equals true + Search Item

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=IsAvailable&Filter.Conditions[0].Operator=Equals&Filter.Conditions[0].Value=true&Search.Value=Item`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"Equals","Value":"true","Values":[],"IsMetaField":false}],"Groups":[]},"Search":{"Value":"Item"},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): TotalCount 3, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":3}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Half Unit Item | True | Available | 0.50 | False |
| 2 | OnHand 4.5 Item | True | Available brian 1 | 4.00 | False |
| 3 | Multi Warehouse Item | True | JLoka-01 | 63.00 | False |

- PASS C2 IsAvailable Equals true + Search Item: 3 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 3

### C3. Filter.Logic OR: StockStatusLabel Equals Back order OR IsAvailable Equals true (two conditions under OR: rejected)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Logic=OR&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=Equals&Filter.Conditions[0].Value=Back%20order&Filter.Conditions[1].Field=IsAvailable&Filter.Conditions[1].Operator=Equals&Filter.Conditions[1].Value=true`

payload: `{"Filter":{"Logic":"OR","Conditions":[{"Field":"StockStatusLabel","Operator":"Equals","Value":"Back order","Values":[],"IsMetaField":false},{"Field":"IsAvailable","Operator":"Equals","Value":"true","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): `CustomInvalidDataException: Filtering by 'StockStatusLabel' is supported only with AND logic.`
- PASS C3 Filter.Logic OR: StockStatusLabel Equals Back order OR IsAvailable Equals true (two conditions under OR: rejected): rejected by the function with CustomInvalidDataException (400): CustomInvalidDataException: Filtering by 'StockStatusLabel' is supported only with AND logic.

### C4. nested AND group: Name Contains Item AND (IsAvailable Equals false)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=Name&Filter.Conditions[0].Operator=Contains&Filter.Conditions[0].Value=Item&Filter.Groups[0].Conditions[0].Field=IsAvailable&Filter.Groups[0].Conditions[0].Operator=Equals&Filter.Groups[0].Conditions[0].Value=false`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"Name","Operator":"Contains","Value":"Item","Values":[],"IsMetaField":false}],"Groups":[{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"Equals","Value":"false","Values":[],"IsMetaField":false}],"Groups":[]}]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- PASS C4 nested AND group: Name Contains Item AND (IsAvailable Equals false): left to the package, the same error as before (InvalidQueryException: Filtering by 'IsAvailable' is not allowed.)

### C5. ProductPrices.UnitPrice GreaterThan 10 + IsAvailable Equals true

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=ProductPrices.UnitPrice&Filter.Conditions[0].Operator=GreaterThan&Filter.Conditions[0].Value=10&Filter.Conditions[1].Field=IsAvailable&Filter.Conditions[1].Operator=Equals&Filter.Conditions[1].Value=true`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"ProductPrices.UnitPrice","Operator":"GreaterThan","Value":"10","Values":[],"IsMetaField":false},{"Field":"IsAvailable","Operator":"Equals","Value":"true","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): TotalCount 7, echo `{"Sort":{"SortBy":"CreateDate","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[{"Field":"ProductPrices.UnitPrice","Operator":"GreaterThan","Value":"10","Values":[],"IsMetaField":false},{"Field":"ProductPrices.PriceListId","Operator":"Equals","Value":"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa","Values":[],"IsMetaField":false}],"Groups":[]},"TotalCount":7}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Multi Warehouse Item | True | JLoka-01 | 63.00 | False |
| 2 | Scrub Suit | True | Available brian 1 | 1.00 | True |
| 3 | matt test child 2 | True | Available | 7.00 | True |
| 4 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 5 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 6 | Loka-01 | True | JLoka-02 | 200.00 | True |
| 7 | Jafar-Product-03 | True | Available | 1000.00 | True |

- PASS C5 ProductPrices.UnitPrice GreaterThan 10 + IsAvailable Equals true: 7 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 7

### S1. Sort IsAvailable desc

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Sort.SortBy=IsAvailable&Sort.Ascending=false`

payload: `{"Sort":{"SortBy":"IsAvailable","Ascending":false,"IsMetaField":false},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Sorting by 'IsAvailable' is not supported.`
- after (the proposal): TotalCount 19, echo `{"TotalCount":19}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Jafar-Product-03 | True | Available | 1000.00 | True |
| 2 | Loka-01 | True | JLoka-02 | 200.00 | True |
| 3 | Test-MetaField | True | JLoka-01 | 80.00 | False |
| 4 | Multi Warehouse Item | True | JLoka-01 | 63.00 | False |
| 5 | Plug | True | JLoka-01 | 50.00 | True |
| 6 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 7 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 8 | matt test child 2 | True | Available | 7.00 | True |
| 9 | iphone caver | True | Available brian 1 | 5.00 | True |
| 10 | OnHand 4.5 Item | True | Available brian 1 | 4.00 | False |
| 11 | Nitrile Gloves Large | True | Available brian 1 | 3.00 | False |
| 12 | Paintbrush | True | Available brian 1 | 2.00 | False |
| 13 | Scrub Suit | True | Available brian 1 | 1.00 | True |
| 14 | Half Unit Item | True | Available | 0.50 | False |
| 15 | Zero Stock Backorder | False | Back order | 0.00 | True |
| 16 | Back Order No Rows | False | Back order | 0.00 | True |
| 17 | Out Of Stock No Rows | False | Out of Stock | 0.00 | False |
| 18 | Committed Exceeds Item | False | Back order | -3.00 | True |
| 19 | Oversold Item | False | Out of Stock | -3.00 | False |

- PASS S1 Sort IsAvailable desc: 19 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 19
- PASS S1: ordered by the available quantity (GetAvailability), descending: 1000.00, 200.00, 80.00, 63.00, 50.00, 30.00, 12.00, 7.00, 5.00, 4.00, 3.00, 2.00, 1.00, 0.50, 0.00, 0.00, 0.00, -3.00, -3.00
<details><summary>SQL</summary>

```sql
DECLARE @__p_0 int = 0;
DECLARE @__p_1 int = 200;

SELECT [t].[Id], [t].[AllowBackorder], [t].[BOMType], [t].[Barcode], [t].[BrandId], [t].[CountryOfOriginId], [t].[CreateDate], [t].[CreatedBy], [t].[Description], [t].[DisplayVariantsAsList], [t].[Height], [t].[HeightUnitId], [t].[InventoryItem], [t].[InventoryUnitOfMeasurementId], [t].[IsSubscribed], [t].[Length], [t].[LengthUnitId], [t].[ManufacturerId], [t].[MaximumOrderQuantity], [t].[MetaFieldSetId], [t].[MinimumOrderQuantity], [t].[Name], [t].[ProductClassId], [t].[ProductState], [t].[ProductType], [t].[ProductVisibility], [t].[PurchasingItem], [t].[QuantityMultiplier], [t].[Remarks], [t].[SKU], [t].[SalesItem], [t].[SeriesId], [t].[SeriesNumber], [t].[ShipTypeId], [t].[ShortDescription], [t].[TaxClassId], [t].[UnitOfMeasurementGroupId], [t].[UpdateDate], [t].[UpdatedBy], [t].[Version], [t].[WarrantyTemplateId], [t].[Weight], [t].[WeightUnitId], [t].[Width], [t].[WidthUnitId], [t0].[Id], [t0].[CreateDate], [t0].[CreatedBy], [t0].[Description], [t0].[Name], [t0].[Rate], [t0].[UpdateDate], [t0].[UpdatedBy], [t0].[Version], [u].[Id], [u].[Active], [u].[Code], [u].[CreateDate], [u].[CreatedBy], [u].[Height], [u].[HeightUnitId], [u].[Length], [u].[LengthUnitId], [u].[Name], [u].[UpdateDate], [u].[UpdatedBy], [u].[Version], [u].[Weight], [u].[WeightUnitId], [u].[Width], [u].[WidthUnitId]
FROM (
    SELECT [p].[Id], [p].[AllowBackorder], [p].[BOMType], [p].[Barcode], [p].[BrandId], [p].[CountryOfOriginId], [p].[CreateDate], [p].[CreatedBy], [p].[Description], [p].[DisplayVariantsAsList], [p].[Height], [p].[HeightUnitId], [p].[InventoryItem], [p].[InventoryUnitOfMeasurementId], [p].[IsSubscribed], [p].[Length], [p].[LengthUnitId], [p].[ManufacturerId], [p].[MaximumOrderQuantity], [p].[MetaFieldSetId], [p].[MinimumOrderQuantity], [p].[Name], [p].[ProductClassId], [p].[ProductState], [p].[ProductType], [p].[ProductVisibility], [p].[PurchasingItem], [p].[QuantityMultiplier], [p].[Remarks], [p].[SKU], [p].[SalesItem], [p].[SeriesId], [p].[SeriesNumber], [p].[ShipTypeId], [p].[ShortDescription], [p].[TaxClassId], [p].[UnitOfMeasurementGroupId], [p].[UpdateDate], [p].[UpdatedBy], [p].[Version], [p].[WarrantyTemplateId], [p].[Weight], [p].[WeightUnitId], [p].[Width], [p].[WidthUnitId], COALESCE((
        SELECT COALESCE(SUM(CAST(CAST([p0].[OnHand] AS bigint) AS DECIMAL(18,2)) - [p0].[Committed]), 0.0)
        FROM [ProductAvailabilities] AS [p0]
        WHERE [p].[Id] = [p0].[ProductId]), 0.0) AS [c]
    FROM [Products] AS [p]
    WHERE [p].[SalesItem] = CAST(1 AS bit) AND [p].[ProductState] = 1 AND [p].[ProductVisibility] <> 20
    ORDER BY COALESCE((
        SELECT COALESCE(SUM(CAST(CAST([p0].[OnHand] AS bigint) AS DECIMAL(18,2)) - [p0].[Committed]), 0.0)
        FROM [ProductAvailabilities] AS [p0]
        WHERE [p].[Id] = [p0].[ProductId]), 0.0) DESC
    OFFSET @__p_0 ROWS FETCH NEXT @__p_1 ROWS ONLY
) AS [t]
LEFT JOIN [TaxClasses] AS [t0] ON [t].[TaxClassId] = [t0].[Id]
LEFT JOIN [UnitOfMeasurements] AS [u] ON [t].[InventoryUnitOfMeasurementId] = [u].[Id]
ORDER BY [t].[c] DESC
```
</details>


### S2. Sort IsAvailable asc

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Sort.SortBy=IsAvailable&Sort.Ascending=true`

payload: `{"Sort":{"SortBy":"IsAvailable","Ascending":true,"IsMetaField":false},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Sorting by 'IsAvailable' is not supported.`
- after (the proposal): TotalCount 19, echo `{"TotalCount":19}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Committed Exceeds Item | False | Back order | -3.00 | True |
| 2 | Oversold Item | False | Out of Stock | -3.00 | False |
| 3 | Out Of Stock No Rows | False | Out of Stock | 0.00 | False |
| 4 | Back Order No Rows | False | Back order | 0.00 | True |
| 5 | Zero Stock Backorder | False | Back order | 0.00 | True |
| 6 | Half Unit Item | True | Available | 0.50 | False |
| 7 | Scrub Suit | True | Available brian 1 | 1.00 | True |
| 8 | Paintbrush | True | Available brian 1 | 2.00 | False |
| 9 | Nitrile Gloves Large | True | Available brian 1 | 3.00 | False |
| 10 | OnHand 4.5 Item | True | Available brian 1 | 4.00 | False |
| 11 | iphone caver | True | Available brian 1 | 5.00 | True |
| 12 | matt test child 2 | True | Available | 7.00 | True |
| 13 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 14 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 15 | Plug | True | JLoka-01 | 50.00 | True |
| 16 | Multi Warehouse Item | True | JLoka-01 | 63.00 | False |
| 17 | Test-MetaField | True | JLoka-01 | 80.00 | False |
| 18 | Loka-01 | True | JLoka-02 | 200.00 | True |
| 19 | Jafar-Product-03 | True | Available | 1000.00 | True |

- PASS S2 Sort IsAvailable asc: 19 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 19
- PASS S2: ordered by the available quantity (GetAvailability), ascending: -3.00, -3.00, 0.00, 0.00, 0.00, 0.50, 1.00, 2.00, 3.00, 4.00, 5.00, 7.00, 12.00, 30.00, 50.00, 63.00, 80.00, 200.00, 1000.00

### S3. Sort StockStatusLabel asc (not a sort of the proposal: the package answers, as before)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Sort.SortBy=StockStatusLabel&Sort.Ascending=true`

payload: `{"Sort":{"SortBy":"StockStatusLabel","Ascending":true,"IsMetaField":false},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Sorting by 'StockStatusLabel' is not supported.`
- after (the proposal): `InvalidQueryException: Sorting by 'StockStatusLabel' is not supported.`
- PASS S3 Sort StockStatusLabel asc (not a sort of the proposal: the package answers, as before): left to the package, the same error as before (InvalidQueryException: Sorting by 'StockStatusLabel' is not supported.)

### S4. Sort StockStatusLabel desc + Filter IsAvailable Equals true (the sort is not supported: as before)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Sort.SortBy=StockStatusLabel&Sort.Ascending=false&Filter.Conditions[0].Field=IsAvailable&Filter.Conditions[0].Operator=Equals&Filter.Conditions[0].Value=true`

payload: `{"Sort":{"SortBy":"StockStatusLabel","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"Equals","Value":"true","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): `InvalidQueryException: Sorting by 'StockStatusLabel' is not supported.`
- PASS S4 Sort StockStatusLabel desc + Filter IsAvailable Equals true (the sort is not supported: as before): the filter is built, the sort is the package's 400 as before (InvalidQueryException: Sorting by 'StockStatusLabel' is not supported.)

### S5. Sort isavailable desc + Filter StockStatusLabel Contains JLoka (filter and sort together)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Sort.SortBy=isavailable&Sort.Ascending=false&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=Contains&Filter.Conditions[0].Value=JLoka`

payload: `{"Sort":{"SortBy":"isavailable","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"Contains","Value":"JLoka","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): TotalCount 6, echo `{"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":6}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Loka-01 | True | JLoka-02 | 200.00 | True |
| 2 | Test-MetaField | True | JLoka-01 | 80.00 | False |
| 3 | Multi Warehouse Item | True | JLoka-01 | 63.00 | False |
| 4 | Plug | True | JLoka-01 | 50.00 | True |
| 5 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 6 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |

- PASS S5 Sort isavailable desc + Filter StockStatusLabel Contains JLoka (filter and sort together): 6 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 6
- PASS S5: ordered by the available quantity (GetAvailability), descending: 200.00, 80.00, 63.00, 50.00, 30.00, 12.00
<details><summary>SQL</summary>

```sql
DECLARE @__text_0_contains nvarchar(100) = N'%JLoka%';
DECLARE @__p_1 int = 0;
DECLARE @__p_2 int = 200;

SELECT [t].[Id], [t].[AllowBackorder], [t].[BOMType], [t].[Barcode], [t].[BrandId], [t].[CountryOfOriginId], [t].[CreateDate], [t].[CreatedBy], [t].[Description], [t].[DisplayVariantsAsList], [t].[Height], [t].[HeightUnitId], [t].[InventoryItem], [t].[InventoryUnitOfMeasurementId], [t].[IsSubscribed], [t].[Length], [t].[LengthUnitId], [t].[ManufacturerId], [t].[MaximumOrderQuantity], [t].[MetaFieldSetId], [t].[MinimumOrderQuantity], [t].[Name], [t].[ProductClassId], [t].[ProductState], [t].[ProductType], [t].[ProductVisibility], [t].[PurchasingItem], [t].[QuantityMultiplier], [t].[Remarks], [t].[SKU], [t].[SalesItem], [t].[SeriesId], [t].[SeriesNumber], [t].[ShipTypeId], [t].[ShortDescription], [t].[TaxClassId], [t].[UnitOfMeasurementGroupId], [t].[UpdateDate], [t].[UpdatedBy], [t].[Version], [t].[WarrantyTemplateId], [t].[Weight], [t].[WeightUnitId], [t].[Width], [t].[WidthUnitId], [t0].[Id], [t0].[CreateDate], [t0].[CreatedBy], [t0].[Description], [t0].[Name], [t0].[Rate], [t0].[UpdateDate], [t0].[UpdatedBy], [t0].[Version], [u].[Id], [u].[Active], [u].[Code], [u].[CreateDate], [u].[CreatedBy], [u].[Height], [u].[HeightUnitId], [u].[Length], [u].[LengthUnitId], [u].[Name], [u].[UpdateDate], [u].[UpdatedBy], [u].[Version], [u].[Weight], [u].[WeightUnitId], [u].[Width], [u].[WidthUnitId]
FROM (
    SELECT [p].[Id], [p].[AllowBackorder], [p].[BOMType], [p].[Barcode], [p].[BrandId], [p].[CountryOfOriginId], [p].[CreateDate], [p].[CreatedBy], [p].[Description], [p].[DisplayVariantsAsList], [p].[Height], [p].[HeightUnitId], [p].[InventoryItem], [p].[InventoryUnitOfMeasurementId], [p].[IsSubscribed], [p].[Length], [p].[LengthUnitId], [p].[ManufacturerId], [p].[MaximumOrderQuantity], [p].[MetaFieldSetId], [p].[MinimumOrderQuantity], [p].[Name], [p].[ProductClassId], [p].[ProductState], [p].[ProductType], [p].[ProductVisibility], [p].[PurchasingItem], [p].[QuantityMultiplier], [p].[Remarks], [p].[SKU], [p].[SalesItem], [p].[SeriesId], [p].[SeriesNumber], [p].[ShipTypeId], [p].[ShortDescription], [p].[TaxClassId], [p].[UnitOfMeasurementGroupId], [p].[UpdateDate], [p].[UpdatedBy], [p].[Version], [p].[WarrantyTemplateId], [p].[Weight], [p].[WeightUnitId], [p].[Width], [p].[WidthUnitId], COALESCE((
        SELECT COALESCE(SUM(CAST(CAST([p3].[OnHand] AS bigint) AS DECIMAL(18,2)) - [p3].[Committed]), 0.0)
        FROM [ProductAvailabilities] AS [p3]
        WHERE [p].[Id] = [p3].[ProductId]), 0.0) AS [c]
    FROM [Products] AS [p]
    WHERE [p].[SalesItem] = CAST(1 AS bit) AND [p].[ProductState] = 1 AND [p].[ProductVisibility] <> 20 AND COALESCE((
        SELECT TOP(1) [c].[Label]
        FROM [CustomStockStatuses] AS [c]
        WHERE [c].[QuantityFrom] <= CAST(COALESCE((
            SELECT COALESCE(SUM(CAST(CAST([p0].[OnHand] AS bigint) AS DECIMAL(18,2)) - [p0].[Committed]), 0.0)
            FROM [ProductAvailabilities] AS [p0]
            WHERE [p].[Id] = [p0].[ProductId]), 0.0) AS int) AND [c].[QuantityTo] >= CAST(COALESCE((
            SELECT COALESCE(SUM(CAST(CAST([p1].[OnHand] AS bigint) AS DECIMAL(18,2)) - [p1].[Committed]), 0.0)
            FROM [ProductAvailabilities] AS [p1]
            WHERE [p].[Id] = [p1].[ProductId]), 0.0) AS int)), CASE
        WHEN COALESCE((
            SELECT COALESCE(SUM(CAST(CAST([p2].[OnHand] AS bigint) AS DECIMAL(18,2)) - [p2].[Committed]), 0.0)
            FROM [ProductAvailabilities] AS [p2]
            WHERE [p].[Id] = [p2].[ProductId]), 0.0) > 0.0 THEN N'Available'
        WHEN [p].[AllowBackorder] = CAST(1 AS bit) THEN N'Back order'
        ELSE N'Out of Stock'
    END) LIKE @__text_0_contains ESCAPE N'\'
    ORDER BY COALESCE((
        SELECT COALESCE(SUM(CAST(CAST([p3].[OnHand] AS bigint) AS DECIMAL(18,2)) - [p3].[Committed]), 0.0)
        FROM [ProductAvailabilities] AS [p3]
        WHERE [p].[Id] = [p3].[ProductId]), 0.0) DESC
    OFFSET @__p_1 ROWS FETCH NEXT @__p_2 ROWS ONLY
) AS [t]
LEFT JOIN [TaxClasses] AS [t0] ON [t].[TaxClassId] = [t0].[Id]
LEFT JOIN [UnitOfMeasurements] AS [u] ON [t].[InventoryUnitOfMeasurementId] = [u].[Id]
ORDER BY [t].[c] DESC
```
</details>


### S6. Sort IsAvailable desc, warehouse W1

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Sort.SortBy=IsAvailable&Sort.Ascending=false&warehouseId=11111111-1111-1111-1111-111111111111`

payload: `{"Sort":{"SortBy":"IsAvailable","Ascending":false,"IsMetaField":false},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Sorting by 'IsAvailable' is not supported.`
- after (the proposal): TotalCount 19, echo `{"TotalCount":19}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Jafar-Product-03 | True | Available | 1000.00 | True |
| 2 | Loka-01 | True | JLoka-02 | 200.00 | True |
| 3 | Test-MetaField | True | JLoka-01 | 80.00 | False |
| 4 | Plug | True | JLoka-01 | 50.00 | True |
| 5 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 6 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 7 | matt test child 2 | True | Available | 7.00 | True |
| 8 | iphone caver | True | Available brian 1 | 5.00 | True |
| 9 | OnHand 4.5 Item | True | Available brian 1 | 4.00 | False |
| 10 | Multi Warehouse Item | True | Available brian 1 | 3.00 | False |
| 11 | Nitrile Gloves Large | True | Available brian 1 | 3.00 | False |
| 12 | Paintbrush | True | Available brian 1 | 2.00 | False |
| 13 | Scrub Suit | True | Available brian 1 | 1.00 | True |
| 14 | Half Unit Item | True | Available | 0.50 | False |
| 15 | Zero Stock Backorder | False | Back order | 0.00 | True |
| 16 | Back Order No Rows | False | Back order | 0.00 | True |
| 17 | Out Of Stock No Rows | False | Out of Stock | 0.00 | False |
| 18 | Committed Exceeds Item | False | Back order | -3.00 | True |
| 19 | Oversold Item | False | Out of Stock | -3.00 | False |

- PASS S6 Sort IsAvailable desc, warehouse W1: 19 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 19
- PASS S6: ordered by the available quantity (GetAvailability), descending: 1000.00, 200.00, 80.00, 50.00, 30.00, 12.00, 7.00, 5.00, 4.00, 3.00, 3.00, 2.00, 1.00, 0.50, 0.00, 0.00, 0.00, -3.00, -3.00

### S7. Sort IsAvailable desc, page 2 of 5

`GET api/Products/shopping-products?pageNumber=2&pageSize=5&Sort.SortBy=IsAvailable&Sort.Ascending=false`

payload: `{"Sort":{"SortBy":"IsAvailable","Ascending":false,"IsMetaField":false},"PageNumber":2,"PageSize":5}`

- before (the author): `InvalidQueryException: Sorting by 'IsAvailable' is not supported.`
- after (the proposal): TotalCount 19, echo `{"TotalCount":19}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 2 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 3 | matt test child 2 | True | Available | 7.00 | True |
| 4 | iphone caver | True | Available brian 1 | 5.00 | True |
| 5 | OnHand 4.5 Item | True | Available brian 1 | 4.00 | False |

- PASS S7 Sort IsAvailable desc, page 2 of 5: page 2 = items 6..10 of the full order, TotalCount 19

### E1. IsAvailable Equals yes

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=IsAvailable&Filter.Conditions[0].Operator=Equals&Filter.Conditions[0].Value=yes`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"Equals","Value":"yes","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): `CustomInvalidDataException: Invalid IsAvailable value: 'yes'. Allowed values are: true, false.`
- PASS E1 IsAvailable Equals yes: rejected by the function with CustomInvalidDataException (400): CustomInvalidDataException: Invalid IsAvailable value: 'yes'. Allowed values are: true, false.

### E2. IsAvailable Equals (no Value)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=IsAvailable&Filter.Conditions[0].Operator=Equals`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"Equals","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): `CustomInvalidDataException: Filtering by 'IsAvailable' with Equals requires a value in 'Value' property.`
- PASS E2 IsAvailable Equals (no Value): rejected by the function with CustomInvalidDataException (400): CustomInvalidDataException: Filtering by 'IsAvailable' with Equals requires a value in 'Value' property.

### E3. IsAvailable GreaterThan true

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=IsAvailable&Filter.Conditions[0].Operator=GreaterThan&Filter.Conditions[0].Value=true`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"GreaterThan","Value":"true","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): `CustomInvalidDataException: IsAvailable can be filtered only with Equals, NotEquals, In or NotIn.`
- PASS E3 IsAvailable GreaterThan true: rejected by the function with CustomInvalidDataException (400): CustomInvalidDataException: IsAvailable can be filtered only with Equals, NotEquals, In or NotIn.

### E4. IsAvailable Contains t

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=IsAvailable&Filter.Conditions[0].Operator=Contains&Filter.Conditions[0].Value=t`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"Contains","Value":"t","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): `CustomInvalidDataException: IsAvailable can be filtered only with Equals, NotEquals, In or NotIn.`
- PASS E4 IsAvailable Contains t: rejected by the function with CustomInvalidDataException (400): CustomInvalidDataException: IsAvailable can be filtered only with Equals, NotEquals, In or NotIn.

### E5. IsAvailable IsNull

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=IsAvailable&Filter.Conditions[0].Operator=IsNull`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"IsNull","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): `CustomInvalidDataException: IsAvailable can be filtered only with Equals, NotEquals, In or NotIn.`
- PASS E5 IsAvailable IsNull: rejected by the function with CustomInvalidDataException (400): CustomInvalidDataException: IsAvailable can be filtered only with Equals, NotEquals, In or NotIn.

### E6. IsAvailable In (no Values)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=IsAvailable&Filter.Conditions[0].Operator=In`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"In","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): `CustomInvalidDataException: Filtering by 'IsAvailable' with In requires a value in 'Values' property.`
- PASS E6 IsAvailable In (no Values): rejected by the function with CustomInvalidDataException (400): CustomInvalidDataException: Filtering by 'IsAvailable' with In requires a value in 'Values' property.

### E7. StockStatusLabel GreaterThan A

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=GreaterThan&Filter.Conditions[0].Value=A`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"GreaterThan","Value":"A","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): `CustomInvalidDataException: StockStatusLabel can be filtered only with Equals, NotEquals, In, NotIn, Contains, StartsWith or EndsWith.`
- PASS E7 StockStatusLabel GreaterThan A: rejected by the function with CustomInvalidDataException (400): CustomInvalidDataException: StockStatusLabel can be filtered only with Equals, NotEquals, In, NotIn, Contains, StartsWith or EndsWith.

### E8. StockStatusLabel Contains (no Value)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=Contains`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"Contains","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): `CustomInvalidDataException: Filtering by 'StockStatusLabel' with Contains requires a value in 'Value' property.`
- PASS E8 StockStatusLabel Contains (no Value): rejected by the function with CustomInvalidDataException (400): CustomInvalidDataException: Filtering by 'StockStatusLabel' with Contains requires a value in 'Value' property.

### E9. Filter.Logic OR: StockStatusLabel Equals JLoka-01 OR Name Contains Plug (stock + column under OR)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Logic=OR&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=Equals&Filter.Conditions[0].Value=JLoka-01&Filter.Conditions[1].Field=Name&Filter.Conditions[1].Operator=Contains&Filter.Conditions[1].Value=Plug`

payload: `{"Filter":{"Logic":"OR","Conditions":[{"Field":"StockStatusLabel","Operator":"Equals","Value":"JLoka-01","Values":[],"IsMetaField":false},{"Field":"Name","Operator":"Contains","Value":"Plug","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): `CustomInvalidDataException: Filtering by 'StockStatusLabel' is supported only with AND logic.`
- PASS E9 Filter.Logic OR: StockStatusLabel Equals JLoka-01 OR Name Contains Plug (stock + column under OR): rejected by the function with CustomInvalidDataException (400): CustomInvalidDataException: Filtering by 'StockStatusLabel' is supported only with AND logic.

### E10. StockStatusColor Equals #73ff00 (not supported: unchanged)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusColor&Filter.Conditions[0].Operator=Equals&Filter.Conditions[0].Value=%2373ff00`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusColor","Operator":"Equals","Value":"#73ff00","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusColor' is not allowed.`
- after (the proposal): `InvalidQueryException: Filtering by 'StockStatusColor' is not allowed.`
- PASS E10 StockStatusColor Equals #73ff00 (not supported: unchanged): left to the package, the same error as before (InvalidQueryException: Filtering by 'StockStatusColor' is not allowed.)

### E11. StockStatusLabel StartsWith (no Value)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=StartsWith`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"StartsWith","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): `CustomInvalidDataException: Filtering by 'StockStatusLabel' with StartsWith requires a value in 'Value' property.`
- PASS E11 StockStatusLabel StartsWith (no Value): rejected by the function with CustomInvalidDataException (400): CustomInvalidDataException: Filtering by 'StockStatusLabel' with StartsWith requires a value in 'Value' property.

### E12. IsAvailable NotEquals (no Value)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Filter.Conditions[0].Field=IsAvailable&Filter.Conditions[0].Operator=NotEquals`

payload: `{"Filter":{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"NotEquals","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): `CustomInvalidDataException: Filtering by 'IsAvailable' with NotEquals requires a value in 'Value' property.`
- PASS E12 IsAvailable NotEquals (no Value): rejected by the function with CustomInvalidDataException (400): CustomInvalidDataException: Filtering by 'IsAvailable' with NotEquals requires a value in 'Value' property.

### X1. Sort ProductPrices.UnitPrice asc + IsAvailable Equals true

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Sort.SortBy=ProductPrices.UnitPrice&Sort.Ascending=true&Filter.Conditions[0].Field=IsAvailable&Filter.Conditions[0].Operator=Equals&Filter.Conditions[0].Value=true`

payload: `{"Sort":{"SortBy":"ProductPrices.UnitPrice","Ascending":true,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[{"Field":"IsAvailable","Operator":"Equals","Value":"true","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'IsAvailable' is not allowed.`
- after (the proposal): TotalCount 14, echo `{"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":14}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Half Unit Item | True | Available | 0.50 | False |
| 2 | Plug | True | JLoka-01 | 50.00 | True |
| 3 | Nitrile Gloves Large | True | Available brian 1 | 3.00 | False |
| 4 | OnHand 4.5 Item | True | Available brian 1 | 4.00 | False |
| 5 | Paintbrush | True | Available brian 1 | 2.00 | False |
| 6 | Test-MetaField | True | JLoka-01 | 80.00 | False |
| 7 | iphone caver | True | Available brian 1 | 5.00 | True |
| 8 | matt test child 2 | True | Available | 7.00 | True |
| 9 | Jafar-Product-03 | True | Available | 1000.00 | True |
| 10 | Multi Warehouse Item | True | JLoka-01 | 63.00 | False |
| 11 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 12 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 13 | Scrub Suit | True | Available brian 1 | 1.00 | True |
| 14 | Loka-01 | True | JLoka-02 | 200.00 | True |

- PASS X1 Sort ProductPrices.UnitPrice asc + IsAvailable Equals true: 14 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 14
- PASS X1: ordered by the price of the price list, ascending (2, 3, 4, 5, 6, 7, 9, 11, 15, 18, 22, 22, 30, 40)

### X2. Sort IsAvailable desc + ProductPrices.UnitPrice GreaterThan 10 (the author's price filter)

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Sort.SortBy=IsAvailable&Sort.Ascending=false&Filter.Conditions[0].Field=ProductPrices.UnitPrice&Filter.Conditions[0].Operator=GreaterThan&Filter.Conditions[0].Value=10`

payload: `{"Sort":{"SortBy":"IsAvailable","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[{"Field":"ProductPrices.UnitPrice","Operator":"GreaterThan","Value":"10","Values":[],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Sorting by 'IsAvailable' is not supported.`
- after (the proposal): TotalCount 9, echo `{"Filter":{"Logic":"AND","Conditions":[{"Field":"ProductPrices.UnitPrice","Operator":"GreaterThan","Value":"10","Values":[],"IsMetaField":false},{"Field":"ProductPrices.PriceListId","Operator":"Equals","Value":"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa","Values":[],"IsMetaField":false}],"Groups":[]},"TotalCount":9}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Jafar-Product-03 | True | Available | 1000.00 | True |
| 2 | Loka-01 | True | JLoka-02 | 200.00 | True |
| 3 | Multi Warehouse Item | True | JLoka-01 | 63.00 | False |
| 4 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 5 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 6 | matt test child 2 | True | Available | 7.00 | True |
| 7 | Scrub Suit | True | Available brian 1 | 1.00 | True |
| 8 | Zero Stock Backorder | False | Back order | 0.00 | True |
| 9 | Back Order No Rows | False | Back order | 0.00 | True |

- PASS X2 Sort IsAvailable desc + ProductPrices.UnitPrice GreaterThan 10 (the author's price filter): 9 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 9
- PASS X2: ordered by the available quantity (GetAvailability), descending: 1000.00, 200.00, 63.00, 30.00, 12.00, 7.00, 1.00, 0.00, 0.00

### X3. Sort ProductPrices.UnitPrice desc + StockStatusLabel In [JLoka-01, Back order]

`GET api/Products/shopping-products?pageNumber=1&pageSize=200&Sort.SortBy=ProductPrices.UnitPrice&Sort.Ascending=false&Filter.Conditions[0].Field=StockStatusLabel&Filter.Conditions[0].Operator=In&Filter.Conditions[0].Values[0]=JLoka-01&Filter.Conditions[0].Values[1]=Back%20order`

payload: `{"Sort":{"SortBy":"ProductPrices.UnitPrice","Ascending":false,"IsMetaField":false},"Filter":{"Logic":"AND","Conditions":[{"Field":"StockStatusLabel","Operator":"In","Values":["JLoka-01","Back order"],"IsMetaField":false}],"Groups":[]},"PageNumber":1,"PageSize":200}`

- before (the author): `InvalidQueryException: Filtering by 'StockStatusLabel' is not allowed.`
- after (the proposal): TotalCount 8, echo `{"Filter":{"Logic":"AND","Conditions":[],"Groups":[]},"TotalCount":8}`

| # | product | isAvailable | stockStatusLabel | available | AllowBackorder |
|---|---|---|---|---|---|
| 1 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 | True |
| 2 | Alex Vando Shirt | True | JLoka-01 | 12.00 | True |
| 3 | Multi Warehouse Item | True | JLoka-01 | 63.00 | False |
| 4 | Zero Stock Backorder | False | Back order | 0.00 | True |
| 5 | Back Order No Rows | False | Back order | 0.00 | True |
| 6 | Committed Exceeds Item | False | Back order | -3.00 | True |
| 7 | Test-MetaField | True | JLoka-01 | 80.00 | False |
| 8 | Plug | True | JLoka-01 | 50.00 | True |

- PASS X3 Sort ProductPrices.UnitPrice desc + StockStatusLabel In [JLoka-01, Back order]: 8 products = the products whose stock status (as GetStockStatus shows it) matches, TotalCount 8
- PASS X3: ordered by the price of the price list, descending (22, 22, 18, 13, 12, 8, 7, 3)

## 3b. The commented alternative: Sort IsAvailable by the custom stock status (its label), then the availability

### Sort IsAvailable desc (alternative)

| # | product | isAvailable | stockStatusLabel | available |
|---|---|---|---|---|
| 1 | Out Of Stock No Rows | False | Out of Stock | 0.00 |
| 2 | Oversold Item | False | Out of Stock | -3.00 |
| 3 | Loka-01 | True | JLoka-02 | 200.00 |
| 4 | Test-MetaField | True | JLoka-01 | 80.00 |
| 5 | Multi Warehouse Item | True | JLoka-01 | 63.00 |
| 6 | Plug | True | JLoka-01 | 50.00 |
| 7 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 |
| 8 | Alex Vando Shirt | True | JLoka-01 | 12.00 |
| 9 | Back Order No Rows | False | Back order | 0.00 |
| 10 | Zero Stock Backorder | False | Back order | 0.00 |
| 11 | Committed Exceeds Item | False | Back order | -3.00 |
| 12 | iphone caver | True | Available brian 1 | 5.00 |
| 13 | OnHand 4.5 Item | True | Available brian 1 | 4.00 |
| 14 | Nitrile Gloves Large | True | Available brian 1 | 3.00 |
| 15 | Paintbrush | True | Available brian 1 | 2.00 |
| 16 | Scrub Suit | True | Available brian 1 | 1.00 |
| 17 | Jafar-Product-03 | True | Available | 1000.00 |
| 18 | matt test child 2 | True | Available | 7.00 |
| 19 | Half Unit Item | True | Available | 0.50 |

- PASS alternative sort desc: every label in one block (Out of Stock | JLoka-02 | JLoka-01 | Back order | Available brian 1 | Available), the quantity descending inside a block, all 19 products
### Sort IsAvailable asc (alternative)

| # | product | isAvailable | stockStatusLabel | available |
|---|---|---|---|---|
| 1 | Half Unit Item | True | Available | 0.50 |
| 2 | matt test child 2 | True | Available | 7.00 |
| 3 | Jafar-Product-03 | True | Available | 1000.00 |
| 4 | Scrub Suit | True | Available brian 1 | 1.00 |
| 5 | Paintbrush | True | Available brian 1 | 2.00 |
| 6 | Nitrile Gloves Large | True | Available brian 1 | 3.00 |
| 7 | OnHand 4.5 Item | True | Available brian 1 | 4.00 |
| 8 | iphone caver | True | Available brian 1 | 5.00 |
| 9 | Committed Exceeds Item | False | Back order | -3.00 |
| 10 | Back Order No Rows | False | Back order | 0.00 |
| 11 | Zero Stock Backorder | False | Back order | 0.00 |
| 12 | Alex Vando Shirt | True | JLoka-01 | 12.00 |
| 13 | Alex Vando Shirt Red | True | JLoka-01 | 30.00 |
| 14 | Plug | True | JLoka-01 | 50.00 |
| 15 | Multi Warehouse Item | True | JLoka-01 | 63.00 |
| 16 | Test-MetaField | True | JLoka-01 | 80.00 |
| 17 | Loka-01 | True | JLoka-02 | 200.00 |
| 18 | Oversold Item | False | Out of Stock | -3.00 |
| 19 | Out Of Stock No Rows | False | Out of Stock | 0.00 |

- PASS alternative sort asc: every label in one block (Available | Available brian 1 | Back order | JLoka-01 | JLoka-02 | Out of Stock), the quantity ascending inside a block, all 19 products
## 4. Two custom ranges holding the same quantity (only through a direct insert: IsValidRecord rejects it)

- products in 25..60 (JLoka-01 and the newer JLoka-01 promo): Alex Vando Shirt Red (30.00) shown as JLoka-01 promo, Plug (50.00) shown as JLoka-01 promo
- StockStatusLabel Contains promo, without OrderByDescending(CreateDate): Alex Vando Shirt Red, Plug (GetStockStatus shows promo for 2)

## 5. Speed (LocalDB; count + page 1 of 50; median of 5 runs)

### 2,000 more products (2 stock rows each)

| request | the author (ms) | the proposal (ms) |
|---|---|---|
| default (CreateDate desc) | 11.8 | 11.6 |
| Sort ProductPrices.UnitPrice asc | 46.1 | 43.7 |
| Filter IsAvailable Equals true | 400 (not supported) | 41.6 |
| Sort IsAvailable desc | 400 (not supported) | 50.5 |
| Sort IsAvailable desc + Filter IsAvailable Equals true | 400 (not supported) | 96.4 |
| Filter StockStatusLabel Contains JLoka | 400 (not supported) | 244.3 |
| Filter StockStatusLabel In [Back order, Out of Stock] | 400 (not supported) | 359.5 |
| Sort IsAvailable desc + Filter StockStatusLabel Contains JLoka | 400 (not supported) | 397.2 |
| Sort IsAvailable desc, the commented alternative (custom stock status, then the availability) | 400 (not supported) | 417.6 |

### 20,000 more products (2 stock rows each)

| request | the author (ms) | the proposal (ms) |
|---|---|---|
| default (CreateDate desc) | 94.1 | 83.5 |
| Sort ProductPrices.UnitPrice asc | 405.8 | 404.3 |
| Filter IsAvailable Equals true | 400 (not supported) | 248.8 |
| Sort IsAvailable desc | 400 (not supported) | 493.3 |
| Sort IsAvailable desc + Filter IsAvailable Equals true | 400 (not supported) | 931.8 |
| Filter StockStatusLabel Contains JLoka | 400 (not supported) | 2286.2 |
| Filter StockStatusLabel In [Back order, Out of Stock] | 400 (not supported) | 1905.4 |
| Sort IsAvailable desc + Filter StockStatusLabel Contains JLoka | 400 (not supported) | 3932.8 |
| Sort IsAvailable desc, the commented alternative (custom stock status, then the availability) | 400 (not supported) | 3581.3 |

