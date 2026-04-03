using MediatR;
using Mini_ETRM.Domain.Entities;
using Mini_ETRM.Domain.Interfaces;

namespace Mini_ETRM.Application.Commands
{
    public class ExecuteTradeCommandHandler : IRequestHandler<ExecuteTradeCommand, Guid>
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly IMarketDataCache _marketDataCache;

        public ExecuteTradeCommandHandler(ITradeRepository tradeRepository, IMarketDataCache marketDataCache)
        {
            _tradeRepository = tradeRepository;
            _marketDataCache = marketDataCache;
        }

        public async Task<Guid> Handle(ExecuteTradeCommand request, CancellationToken cancellationToken)
        {
            // Get the exact price of this millisecond from the cache
            var latestTick = _marketDataCache.GetLatestTick(request.Commodity);

            if (latestTick == null)
            {
                throw new InvalidOperationException($"No hay datos de mercado disponibles para {request.Commodity}");
            }

            // Create the Domain entity
            var trade = new Trade(
                id: Guid.NewGuid(),
                commodity: request.Commodity,
                type: request.Type,
                volume: request.Volume,
                executionPrice: latestTick.Price, // Gets the exact price of this millisecond from the cache
                timestamp: DateTimeOffset.UtcNow
            );

            // Persist in database
            await _tradeRepository.AddAsync(trade, cancellationToken);

            return trade.Id;
        }
    }
}