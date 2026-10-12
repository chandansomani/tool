namespace backend.Conversions;

public sealed class ConversionService : IConversionService
{
    private sealed record UnitDefinition(decimal Factor, decimal Offset = 0);

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

    private static readonly IReadOnlyDictionary<string, UnitDefinition> TemperatureUnits =
        new Dictionary<string, UnitDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            ["celsius"] = new(1m),
            ["fahrenheit"] = new(5m / 9m, -160m / 9m),
            ["kelvin"] = new(1m, -273.15m)
        };

    private static readonly IReadOnlyDictionary<string, UnitDefinition> MassUnits =
        new Dictionary<string, UnitDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            ["kilogram"] = new(1m),
            ["gram"] = new(0.001m),
            ["milligram"] = new(0.000001m),
            ["metric-ton"] = new(1000m),
            ["ounce"] = new(0.028349523125m),
            ["pound"] = new(0.45359237m)
        };

    private static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, UnitDefinition>> Categories =
        new Dictionary<string, IReadOnlyDictionary<string, UnitDefinition>>(StringComparer.OrdinalIgnoreCase)
        {
            ["length"] = LengthUnits,
            ["temperature"] = TemperatureUnits,
            ["mass"] = MassUnits,
            ["weight"] = MassUnits
        };

    public ConversionResult Convert(ConversionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Category) || !Categories.TryGetValue(request.Category, out var units))
        {
            return new(null, "Unknown category. Supported categories are length, temperature, and mass (also accepted as weight).");
        }

        if (string.IsNullOrWhiteSpace(request.FromUnit))
        {
            return new(null, "A source unit is required.");
        }

        if (!units.TryGetValue(request.FromUnit, out var fromUnit))
        {
            return new(null, $"Unknown source unit '{request.FromUnit}' for category '{request.Category}'.");
        }

        if (string.IsNullOrWhiteSpace(request.ToUnit))
        {
            return new(null, "A target unit is required.");
        }

        if (!units.TryGetValue(request.ToUnit, out var toUnit))
        {
            return new(null, $"Unknown target unit '{request.ToUnit}' for category '{request.Category}'.");
        }

        try
        {
            var baseValue = request.Value * fromUnit.Factor + fromUnit.Offset;
            var convertedValue = decimal.Round(
                (baseValue - toUnit.Offset) / toUnit.Factor,
                12,
                MidpointRounding.ToEven);

            return new(
                new ConversionResponse(request.Category, request.Value, request.FromUnit, convertedValue, request.ToUnit),
                null);
        }
        catch (OverflowException)
        {
            return new(null, "The value is outside the supported conversion range.");
        }
    }

    public IReadOnlyList<ConversionCategoryResponse> GetCategories() =>
    [
        new("length", LengthUnits.Keys.Order(StringComparer.OrdinalIgnoreCase).ToArray()),
        new("temperature", TemperatureUnits.Keys.Order(StringComparer.OrdinalIgnoreCase).ToArray()),
        new("mass", MassUnits.Keys.Order(StringComparer.OrdinalIgnoreCase).ToArray())
    ];
}