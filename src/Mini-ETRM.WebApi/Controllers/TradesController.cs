using MediatR;
using Microsoft.AspNetCore.Mvc;
using Mini_ETRM.Application.Commands;
using Mini_ETRM.Application.Queries;

namespace Mini_ETRM.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TradesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TradesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> ExecuteTrade([FromBody] ExecuteTradeCommand command)
    {
        try
        {
            var tradeId = await _mediator.Send(command);
            // Retornamos 201 Created con el ID generado
            return CreatedAtAction(nameof(GetTradePnL), new { id = tradeId }, new { TradeId = tradeId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}/pnl")]
    public async Task<IActionResult> GetTradePnL(Guid id)
    {
        try
        {
            var query = new GetTradePnLQuery(id);
            var pnlDto = await _mediator.Send(query);
            return Ok(pnlDto);
        }
        catch (Exception ex)
        {
            // In a real case, it would be ideal to handle specific exceptions (e.g., NotFoundException) to return more precise HTTP codes
            return NotFound(new { error = ex.Message });
        }
    }
}