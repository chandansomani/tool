namespace backend.Conversions;

public interface IConversionService
{
    ConversionResult Convert(ConversionRequest request);

    IReadOnlyList<ConversionCategoryResponse> GetCategories();
}