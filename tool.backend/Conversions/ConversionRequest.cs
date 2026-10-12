namespace backend.Conversions;

/// <summary>Describes a value and the units to convert between.</summary>
public sealed record ConversionRequest
{
    /// <summary>The conversion category: length, temperature, or mass.</summary>
    public required string Category { get; init; }

    /// <summary>The source unit identifier.</summary>
    public required string FromUnit { get; init; }

    /// <summary>The target unit identifier.</summary>
    public required string ToUnit { get; init; }

    /// <summary>The numeric value to convert.</summary>
    public decimal Value { get; init; }
}