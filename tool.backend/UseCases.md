# API Use Cases

This document demonstrates the main use cases for the Unit Conversion API. The service exposes:

- `GET /api/v1/units`
- `POST /api/v1/conversions`

The examples below use the local application URL:

```text
http://localhost:5250
```

---

## 1. List supported categories and units

### Request

```http
GET /api/v1/units HTTP/1.1
Host: localhost:5250
Accept: application/json
```

### Example response

```json
[
  {
    "category": "length",
    "units": [
      "centimeter",
      "foot",
      "inch",
      "kilometer",
      "meter",
      "mile",
      "millimeter",
      "yard"
    ]
  },
  {
    "category": "temperature",
    "units": [
      "celsius",
      "fahrenheit",
      "kelvin"
    ]
  },
  {
    "category": "mass",
    "units": [
      "gram",
      "kilogram",
      "metric-ton",
      "milligram",
      "ounce",
      "pound"
    ]
  }
]
```

Use this call when the client needs to show the supported conversion options before prompting the user.

---

## 2. Convert length: meters to feet

### Request

```http
POST /api/v1/conversions HTTP/1.1
Host: localhost:5250
Content-Type: application/json

{
  "category": "length",
  "fromUnit": "meter",
  "toUnit": "foot",
  "value": 1
}
```

### Example response

```json
{
  "category": "length",
  "value": 1,
  "fromUnit": "meter",
  "convertedValue": 3.280839895013,
  "toUnit": "foot"
}
```

This is a typical scenario for a UI that converts distances such as room dimensions, travel distances, or construction measurements.

---

## 3. Convert temperature: Fahrenheit to Celsius

### Request

```http
POST /api/v1/conversions HTTP/1.1
Host: localhost:5250
Content-Type: application/json

{
  "category": "temperature",
  "fromUnit": "fahrenheit",
  "toUnit": "celsius",
  "value": 98.6
}
```

### Example response

```json
{
  "category": "temperature",
  "value": 98.6,
  "fromUnit": "fahrenheit",
  "convertedValue": 37,
  "toUnit": "celsius"
}
```

This is useful for weather apps, medical tools, or reports that need to switch between temperature scales.

---

## 4. Convert mass: kilograms to pounds

### Request

```http
POST /api/v1/conversions HTTP/1.1
Host: localhost:5250
Content-Type: application/json

{
  "category": "mass",
  "fromUnit": "kilogram",
  "toUnit": "pound",
  "value": 1
}
```

### Example response

```json
{
  "category": "mass",
  "value": 1,
  "fromUnit": "kilogram",
  "convertedValue": 2.204622621849,
  "toUnit": "pound"
}
```

This supports scenarios like shipping weight conversion, fitness tracking, or ingredient measurements.

---

## 5. Invalid conversion request

### Request

```http
POST /api/v1/conversions HTTP/1.1
Host: localhost:5250
Content-Type: application/json

{
  "category": "length",
  "fromUnit": "meter",
  "toUnit": "lightyear",
  "value": 1
}
```

### Example response

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Unknown target unit 'lightyear' for category 'length'."
}
```

The API returns `400 Bad Request` when the category, source unit, or target unit is invalid.

---

## Notes

- Category names and unit names are case-insensitive.
- Results are rounded to 12 decimal places.
- `weight` is also accepted as an alias for `mass`.
- All requests use JSON payloads and return JSON responses.
