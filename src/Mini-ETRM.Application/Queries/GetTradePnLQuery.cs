using MediatR;
using Mini_ETRM.Application.DTOs;

namespace Mini_ETRM.Application.Queries
{
    public record GetTradePnLQuery(Guid TradeId) : IRequest<TradePnLDto>;

}