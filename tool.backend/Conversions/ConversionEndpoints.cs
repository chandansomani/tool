using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace backend.Conversions;

public static class ConversionEndpoints
{
    public static WebApplication MapConversionEndpoints(this WebApplication app)
    {
        app.MapPost("/api/v1/conversions", Convert)
            .WithName("ConvertUnits")
            .WithSummary("Convert a value between units")
            .WithDescription("Converts between supported length, temperature, and mass units.")
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        app.MapGet("/api/v1/units", GetCategories)
            .WithName("GetSupportedUnits")
            .WithSummary("List supported conversion categories and units")
            .WithDescription("Returns the category and unit identifiers accepted by the conversion endpoint.");

        return app;
    }

    private static Results<Ok<ConversionResponse>, BadRequest<ProblemDetails>> Convert(
        ConversionRequest request,
        IConversionService conversionService)
    {
        var result = conversionService.Convert(request);
        if (!result.IsSuccess)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid conversion",
                Detail = result.Error
            });
        }

        return TypedResults.Ok(result.Response!);
    }

    private static Ok<IReadOnlyList<ConversionCategoryResponse>> GetCategories(IConversionService conversionService) =>
        TypedResults.Ok(conversionService.GetCategories());
}