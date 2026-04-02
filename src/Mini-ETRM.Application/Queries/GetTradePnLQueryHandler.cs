using MediatR;
using Mini_ETRM.Domain.Enums;
using Mini_ETRM.Domain.Interfaces;
using Mini_ETRM.Application.DTOs;

namespace Mini_ETRM.Application.Queries
{
    public class GetTradePnLQueryHandler : IRequestHandler<GetTradePnLQuery, TradePnLDto>
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly IMarketDataCache _marketDataCache;

        public GetTradePnLQueryHandler(ITradeRepository tradeRepository, IMarketDataCache marketDataCache)
        {
            _tradeRepository = tradeRepository;
            _marketDataCache = marketDataCache;
        }

        public async Task<TradePnLDto> Handle(GetTradePnLQuery request, CancellationToken cancellationToken)
        {
            // 1. Buscar el Trade histórico en la base de datos
            var trade = await _tradeRepository.GetByIdAsync(request.TradeId, cancellationToken);

            if (trade == null)
            {
                // En un caso real usaríamos una excepción de dominio personalizada, ej: NotFoundException
                throw new Exception($"Trade con ID {request.TradeId} no encontrado.");
            }

            // 2. Obtener el precio del mercado en vivo
            var latestTick = _marketDataCache.GetLatestTick(trade.Commodity);

            if (latestTick == null)
            {
                throw new InvalidOperationException($"No hay datos de mercado disponibles para {trade.Commodity}");
            }

            // 3. Calcular el P&L
            decimal currentPrice = latestTick.Price;
            decimal pnl = 0m;

            if (trade.Type == TradeType.Buy)
            {
                pnl = (currentPrice - trade.ExecutionPrice) * trade.Volume;
            }
            else if (trade.Type == TradeType.Sell)
            {
                pnl = (trade.ExecutionPrice - currentPrice) * trade.Volume;
            }

            // 4. Retornar el DTO
            return new TradePnLDto(
                TradeId: trade.Id,
                Commodity: trade.Commodity,
                Type: trade.Type,
                Volume: trade.Volume,
                ExecutionPrice: trade.ExecutionPrice,
                CurrentMarketPrice: currentPrice,
                ProfitAndLoss: pnl,
                CalculationTime: DateTimeOffset.UtcNow
            );
        }
    }
}