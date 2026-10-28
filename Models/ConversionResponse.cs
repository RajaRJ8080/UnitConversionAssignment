namespace UnitConversionApi.Models;

public sealed class ConversionResponse
{
    public double OriginalValue { get; set; }
    public string Category { get; set; } = string.Empty;
    public string FromUnit { get; set; } = string.Empty;
    public string ToUnit { get; set; } = string.Empty;
    public double ConvertedValue { get; set; }
    public string Message { get; set; } = string.Empty;
}
