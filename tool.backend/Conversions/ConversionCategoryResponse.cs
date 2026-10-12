namespace backend.Conversions;

/// <summary>Lists the unit identifiers supported by a conversion category.</summary>
public sealed record ConversionCategoryResponse(string Category, IReadOnlyList<string> Units);