using Currencies.Core.Interface;
using Currencies.Core.Model;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Currencies.Server.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class CurrencyController(ICurrencyRepository currencyRepository, ILogger<CurrencyController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Currency>>> GetLast()
    {
        try
        {
            var currencies = await currencyRepository.GetLastAsync();
            return Ok(currencies);
        } catch(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
        }
    }


    [HttpGet]
   public async Task<ActionResult<Currency>> GetExchanges([FromQuery][Required] string currencyCode, [FromQuery][Required] DateTime dateFrom, [FromQuery][Required] DateTime dateTo)
    {
        try
        {
            var currency = await currencyRepository.GetAsync(currencyCode, dateFrom, dateTo);
            return currency != null ? Ok(currency) : BadRequest($"Currency with {currencyCode} not found!");
        } catch(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<Currency>>> GetAll()
    {
        try
        {
            var currencies = await currencyRepository.GetAllAsync();
            return Ok(currencies);
        } catch(Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong!");
        }
    }
}