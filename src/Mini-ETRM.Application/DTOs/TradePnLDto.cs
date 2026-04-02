using Mini_ETRM.Domain.Enums;

namespace Mini_ETRM.Application.DTOs
{
    public record TradePnLDto(
        Guid TradeId,
        Commodity Commodity,
        TradeType Type,
        decimal Volume,
        decimal ExecutionPrice,
        decimal CurrentMarketPrice,
        decimal ProfitAndLoss,
        DateTimeOffset CalculationTime
    );
}
