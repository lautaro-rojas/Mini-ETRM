using Microsoft.AspNetCore.Mvc;
using Mini_ETRM.Domain.Enums;
using Mini_ETRM.Domain.Interfaces;

namespace Mini_ETRM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarketDataController : ControllerBase
{
    private readonly IMarketDataCache _marketDataCache;

    public MarketDataController(IMarketDataCache marketDataCache)
    {
        _marketDataCache = marketDataCache;
    }

    [HttpGet("latest")]
    public IActionResult GetLatestPrices()
    {
        var wti = _marketDataCache.GetLatestTick(Commodity.WTI);
        var brent = _marketDataCache.GetLatestTick(Commodity.Brent);

        if (wti == null || brent == null)
        {
            return StatusCode(503, new { error = "The market simulator has not generated prices yet." });
        }

        return Ok(new
        {
            WTI = wti,
            Brent = brent,
            Timestamp = DateTimeOffset.UtcNow
        });
    }
}