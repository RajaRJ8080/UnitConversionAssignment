using System.ComponentModel.DataAnnotations;

namespace UnitConversionApi.Models;

public sealed class ConversionRequest
{
    [Required]
    public string Category { get; set; } = string.Empty;

    public double Value { get; set; }

    [Required]
    public string FromUnit { get; set; } = string.Empty;

    [Required]
    public string ToUnit { get; set; } = string.Empty;
}
