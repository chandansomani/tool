namespace backend.Conversions;

/// <summary>Contains either a conversion response or a client-safe validation error.</summary>
public sealed record ConversionResult(ConversionResponse? Response, string? Error)
{
    public bool IsSuccess => Response is not null;
}