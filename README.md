# Unit Conversion API (.NET 8)

Run simple ASP.NET Core Web API following a controller, interface, and service pattern.

## Requirements
- .NET 8 SDK
- Visual Studio 2022 with the ASP.NET and web development workload

## Run
1. Open Visual Studio 2022.
2. Open `UnitConversionApi.csproj` in Visual Studio 2022.
3. Install NuGet packages (if necessary).
4. Run the project. Swagger should open in Development.
5. If needed, open `http://localhost:7080/swagger` or configure you url in launchsetting.json.

## Convert units
`POST /api/Conversion/Convert`

Length payload:
```json
{
  "category": "length",
  "value": 2,
  "fromUnit": "km",
  "toUnit": "m"
}
```

Temperature payload:
```json
{
  "category": "temperature",
  "value": 100,
  "fromUnit": "C",
  "toUnit": "F"
}
```

Mass payload:
```json
{
  "category": "mass",
  "value": 1,
  "fromUnit": "kg",
  "toUnit": "lb"
}
```



`GET /api/Conversion/GetSupportedUnits`  Return list supported units.

Here Invalid categories, invalid units, missing required strings, non-finite values, and temperatures below absolute zero are rejected. Unexpected errors return HTTP 500.

