# Developer Guide: Adding New Conversion Units

This guide explains how to extend the conversion API when you need to support additional units or a new conversion category.

## Where the conversion rules live

The conversion logic is centralized in [Conversions/ConversionService.cs](Conversions/ConversionService.cs). The service defines:

- unit conversion tables for each category
- the category-to-unit mapping
- the response shape used by the API
- the list returned by `GET /api/v1/units`

The conversion model is based on the formula:

```text
baseValue = value * factor + offset
convertedValue = (baseValue - toUnit.Offset) / toUnit.Factor
```

This means each unit is expressed as a normalized factor and optional offset.

---

## Case 1: Add a new unit to an existing category

Example: add `yard` to the `length` category.

### Step 1: Add the unit definition

Open [Conversions/ConversionService.cs](Conversions/ConversionService.cs) and update the relevant dictionary:

```csharp
private static readonly IReadOnlyDictionary<string, UnitDefinition> LengthUnits =
    new Dictionary<string, UnitDefinition>(StringComparer.OrdinalIgnoreCase)
    {
        ["meter"] = new(1m),
        ["kilometer"] = new(1000m),
        ["centimeter"] = new(0.01m),
        ["millimeter"] = new(0.001m),
        ["inch"] = new(0.0254m),
        ["foot"] = new(0.3048m),
        ["yard"] = new(0.9144m),
        ["mile"] = new(1609.344m)
    };
```

### Step 2: Keep the identifiers consistent

- Use lowercase unit names.
- Use `StringComparer.OrdinalIgnoreCase` so requests like `Meter`, `METER`, and `meter` all work.
- Prefer standard identifiers that match business expectations.

### Step 3: Verify the category list

If the unit is added to an existing category, it is automatically included when `GetCategories()` builds the response.

---

## Case 2: Add a completely new conversion category

Example: add `volume` with units like `liter`, `milliliter`, and `gallon`.

### Step 1: Create a new dictionary

Add a unit dictionary in the same style as `LengthUnits` and `MassUnits`:

```csharp
private static readonly IReadOnlyDictionary<string, UnitDefinition> VolumeUnits =
    new Dictionary<string, UnitDefinition>(StringComparer.OrdinalIgnoreCase)
    {
        ["liter"] = new(1m),
        ["milliliter"] = new(0.001m),
        ["gallon"] = new(3.785411784m)
    };
```

### Step 2: Register the category

Add the new category to the `Categories` dictionary in `ConversionService`:

```csharp
private static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, UnitDefinition>> Categories =
    new Dictionary<string, IReadOnlyDictionary<string, UnitDefinition>>(StringComparer.OrdinalIgnoreCase)
    {
        ["length"] = LengthUnits,
        ["temperature"] = TemperatureUnits,
        ["mass"] = MassUnits,
        ["weight"] = MassUnits,
        ["volume"] = VolumeUnits
    };
```

### Step 3: Expose the category in the unit list

Update `GetCategories()`:

```csharp
public IReadOnlyList<ConversionCategoryResponse> GetCategories() =>
[
    new("length", LengthUnits.Keys.Order(StringComparer.OrdinalIgnoreCase).ToArray()),
    new("temperature", TemperatureUnits.Keys.Order(StringComparer.OrdinalIgnoreCase).ToArray()),
    new("mass", MassUnits.Keys.Order(StringComparer.OrdinalIgnoreCase).ToArray()),
    new("volume", VolumeUnits.Keys.Order(StringComparer.OrdinalIgnoreCase).ToArray())
];
```

This ensures the API exposes the new category through `GET /api/v1/units`.

---

## Important rules for new units

### Use correct conversion factors

Every unit must be defined relative to the category base unit.

For example:

- `meter` = 1
- `kilometer` = 1000
- `millimeter` = 0.001

If a conversion is not linear, use `Offset` to handle the relationship.

Example for temperature:

```csharp
["fahrenheit"] = new(5m / 9m, -160m / 9m)
["kelvin"] = new(1m, -273.15m)
```

### Keep identifiers stable

Use a clear, consistent naming convention:

- lowercase strings
- hyphenated names when needed (`metric-ton`)
- alias support only when it adds value (`weight` as alias for `mass`)

### Validate behavior before shipping

Test common values and known conversions such as:

- `1 meter -> 3.280839895013 foot`
- `32 fahrenheit -> 0 celsius`
- `1 kilogram -> 2.204622621849 pound`

---

## Recommended developer checklist

Before finishing a new unit addition, confirm:

- [ ] the unit is added to the correct category dictionary
- [ ] the category is registered in `Categories`
- [ ] the new unit appears in `GetCategories()`
- [ ] the conversion formula is correct
- [ ] casing and aliases behave as expected
- [ ] the API still builds successfully

---

## Validation step

Run the project build after adding units:

```powershell
dotnet build
```

Then test the endpoint using the sample requests in [backend.http](backend.http) or the examples in [UseCases.md](UseCases.md).

---

## Summary

Adding a new unit is usually a small change in one place: the unit dictionary for that category. If you need a new category, you also add it to the category registry and the `GetCategories()` output.

The API is intentionally centralized around the conversion service, which makes new unit support straightforward and predictable.
