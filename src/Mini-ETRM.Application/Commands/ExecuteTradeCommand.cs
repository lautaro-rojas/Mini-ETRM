using MediatR;
using Mini_ETRM.Domain.Enums;

namespace Mini_ETRM.Application.Commands
{
    // The command returns the Guid of the generated Trade
    public record ExecuteTradeCommand(
        Commodity Commodity,
        TradeType Type,
        decimal Volume
    ) : IRequest<Guid>;
}