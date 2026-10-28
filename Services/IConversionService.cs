using UnitConversionApi.Models;

namespace UnitConversionApi.Services;

public interface IConversionService
{
    Task<ConversionResponse> ConvertAsync(ConversionRequest request);
}
