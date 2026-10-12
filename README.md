# Unit Conversion API

A ASP.NET Core 10 REST API for converting length, temperature, and mass values. Unit definitions are hard-coded and grouped behind a conversion service so new categories can be added without changing the HTTP endpoints.

## Run

From this directory:

```powershell
dotnet run
```

The HTTP launch profile listens on `http://localhost:5250`. In Development, the OpenAPI document is available at `http://localhost:5250/openapi/v1.json`.

## Endpoints

### `GET /api/v1/units`

Lists supported categories and unit identifiers. Categories are `length`, `temperature`, and `mass`; `weight` is accepted as an alias for `mass` in conversion requests.

### `POST /api/v1/conversions`

Request:

```json
{
  "category": "length",
  "fromUnit": "meter",
  "toUnit": "foot",
  "value": 1
}
```

Successful response:

```json
{
  "category": "length",
  "value": 1,
  "fromUnit": "meter",
  "convertedValue": 3.280839895013,
  "toUnit": "foot"
}
```

Category and unit identifiers are case-insensitive. Results are rounded to 12 decimal places. Invalid categories, unsupported units, and values that overflow the decimal conversion range return `400 Bad Request` with a Problem Details response.

## Supported units

- Length: `meter`, `kilometer`, `centimeter`, `millimeter`, `inch`, `foot`, `yard`, `mile`
- Temperature: `celsius`, `fahrenheit`, `kelvin`
- Mass (`weight` alias): `kilogram`, `gram`, `milligram`, `metric-ton`, `ounce`, `pound`

## Use cases

See [UseCases.md](UseCases.md) for sample client scenarios, request payloads, and example responses for successful and invalid conversions.

## Adding new units

Follow [AddingNewConversions.md](AddingNewConversions.md) when you need to add a new conversion unit or category to the API.

## Project layout

```text
backend/
  Conversions/
    ConversionEndpoints.cs    HTTP routes and OpenAPI metadata
    ConversionRequest.cs      Request contract
    ConversionResponse.cs     Response contract
    ConversionCategoryResponse.cs
    ConversionResult.cs       Service result and validation error
    IConversionService.cs     Extension point for conversion behavior
    ConversionService.cs      Unit definitions and conversion logic
  Program.cs                  Application setup and dependency registration
  backend.http                Example endpoint requests
  README.md                   API and project documentation
```

To add a category, define its unit factors and offsets in `ConversionService`, add it to the category catalog, and list it in `GetCategories`. Each unit converts through the category's base unit using `baseValue = value * factor + offset`.

## Verify

```powershell
dotnet build
```

The `backend.http` file contains sample calls for both endpoints and a bad-request case.
