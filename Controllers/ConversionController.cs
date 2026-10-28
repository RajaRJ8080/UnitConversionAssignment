using Microsoft.AspNetCore.Mvc;
using UnitConversionApi.Models;
using UnitConversionApi.Services;

namespace UnitConversionApi.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public sealed class ConversionController : ControllerBase
{
    private readonly IConversionService _conversionService;

    public ConversionController(IConversionService conversionService)
    {
        _conversionService = conversionService;
    }

    [HttpPost]
    public async Task<IActionResult> ConvertTheUnit(ConversionRequest request)
    {
        try
        {
            var response = await _conversionService.ConvertAsync(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Internal Server Error" });
        }
    }

    [HttpGet]
    public IActionResult GetSupportedUnits()
    {
        return Ok(new
        {
            length = new[] { "m", "km", "cm", "mm", "mi", "yd", "ft", "in" },
            mass = new[] { "kg", "g", "mg", "t", "lb", "oz" },
            temperature = new[] { "C", "F", "K" }
        });
    }
}
