using MediatR;
using Mini_ETRM.Domain.Enums;

namespace Mini_ETRM.Application.Commands
{
    // El comando devuelve el Guid del Trade generado
    public record ExecuteTradeCommand(
        Commodity Commodity,
        TradeType Type,
        decimal Volume
    ) : IRequest<Guid>;
}