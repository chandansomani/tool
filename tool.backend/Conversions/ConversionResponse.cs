namespace backend.Conversions;

/// <summary>Represents a converted value and its source and target units.</summary>
public sealed record ConversionResponse(
    string Category,
    decimal Value,
    string FromUnit,
    decimal ConvertedValue,
    string ToUnit);