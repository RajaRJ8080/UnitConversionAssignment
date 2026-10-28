using UnitConversionApi.Models;

namespace UnitConversionApi.Services;

public sealed class ConversionService : IConversionService
{
    private readonly ILogger<ConversionService> _logger;

    private static readonly Dictionary<string, double> LengthFactors = new(StringComparer.OrdinalIgnoreCase)
    {
        ["m"] = 1, ["meter"] = 1, ["meters"] = 1,
        ["km"] = 1000, ["kilometer"] = 1000, ["kilometers"] = 1000,
        ["cm"] = 0.01, ["centimeter"] = 0.01, ["centimeters"] = 0.01,
        ["mm"] = 0.001, ["millimeter"] = 0.001, ["millimeters"] = 0.001,
        ["mi"] = 1609.344, ["mile"] = 1609.344, ["miles"] = 1609.344,
        ["yd"] = 0.9144, ["yard"] = 0.9144, ["yards"] = 0.9144,
        ["ft"] = 0.3048, ["foot"] = 0.3048, ["feet"] = 0.3048,
        ["in"] = 0.0254, ["inch"] = 0.0254, ["inches"] = 0.0254
    };

    private static readonly Dictionary<string, double> MassFactors = new(StringComparer.OrdinalIgnoreCase)
    {
        ["kg"] = 1, ["kilogram"] = 1, ["kilograms"] = 1,
        ["g"] = 0.001, ["gram"] = 0.001, ["grams"] = 0.001,
        ["mg"] = 0.000001, ["milligram"] = 0.000001, ["milligrams"] = 0.000001,
        ["t"] = 1000, ["tonne"] = 1000, ["tonnes"] = 1000,
        ["lb"] = 0.45359237, ["lbs"] = 0.45359237, ["pound"] = 0.45359237, ["pounds"] = 0.45359237,
        ["oz"] = 0.028349523125, ["ounce"] = 0.028349523125, ["ounces"] = 0.028349523125
    };

    public ConversionService(ILogger<ConversionService> logger)
    {
        _logger = logger;
    }

    public Task<ConversionResponse> ConvertAsync(ConversionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            if (double.IsNaN(request.Value) || double.IsInfinity(request.Value))
                throw new ArgumentException("Value must be a finite number.");

            if (string.IsNullOrWhiteSpace(request.Category) ||
                string.IsNullOrWhiteSpace(request.FromUnit) ||
                string.IsNullOrWhiteSpace(request.ToUnit))
                throw new ArgumentException("Category, fromUnit, and toUnit are required.");

            var category = request.Category.Trim().ToLowerInvariant();
            var from = request.FromUnit.Trim();
            var to = request.ToUnit.Trim();

            double convertedValue = category switch
            {
                "length" => ConvertLinear(request.Value, from, to, LengthFactors, "length"),
                "mass" => ConvertLinear(request.Value, from, to, MassFactors, "mass"),
                "temperature" => ConvertTemperature(request.Value, from, to),
                _ => throw new ArgumentException("Unsupported category. Use length, mass, or temperature.")
            };

            if (double.IsNaN(convertedValue) || double.IsInfinity(convertedValue))
                throw new ArgumentException("Conversion result is outside the supported numeric range.");

            var response = new ConversionResponse
            {
                OriginalValue = request.Value,
                Category = category,
                FromUnit = from,
                ToUnit = to,
                ConvertedValue = Math.Round(convertedValue, 8, MidpointRounding.AwayFromZero),
                Message = "Conversion successful"
            };

            return Task.FromResult(response);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while converting units.");
            throw;
        }
    }

    private static double ConvertLinear(
        double value,
        string from,
        string to,
        Dictionary<string, double> factors,
        string category)
    {
        if (!factors.TryGetValue(from, out var fromFactor))
            throw new ArgumentException($"Unsupported {category} unit '{from}'.");
        if (!factors.TryGetValue(to, out var toFactor))
            throw new ArgumentException($"Unsupported {category} unit '{to}'.");

        return value * fromFactor / toFactor;
    }

    private static double ConvertTemperature(double value, string from, string to)
    {
        var source = NormalizeTemperatureUnit(from);
        var target = NormalizeTemperatureUnit(to);

        double celsius = source switch
        {
            "c" => value,
            "f" => (value - 32) * 5 / 9,
            "k" => value - 273.15,
            _ => throw new ArgumentException($"Unsupported temperature unit '{from}'. Use C, F, or K.")
        };

        if ((source == "c" && value < -273.15) ||
            (source == "f" && value < -459.67) ||
            (source == "k" && value < 0))
            throw new ArgumentException("Temperature cannot be below absolute zero.");

        return target switch
        {
            "c" => celsius,
            "f" => celsius * 9 / 5 + 32,
            "k" => celsius + 273.15,
            _ => throw new ArgumentException($"Unsupported temperature unit '{to}'. Use C, F, or K.")
        };
    }

    private static string NormalizeTemperatureUnit(string unit) =>
        unit.Trim().ToLowerInvariant() switch
        {
            "c" or "°c" or "celsius" => "c",
            "f" or "°f" or "fahrenheit" => "f",
            "k" or "kelvin" => "k",
            _ => "?"
        };
}
