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
            // 1. Obtener el precio exacto de este milisegundo desde la caché O(1)
            var latestTick = _marketDataCache.GetLatestTick(request.Commodity);

            if (latestTick == null)
            {
                throw new InvalidOperationException($"No hay datos de mercado disponibles para {request.Commodity}");
            }

            // 2. Crear la entidad de Dominio
            var trade = new Trade(
                id: Guid.NewGuid(),
                commodity: request.Commodity,
                type: request.Type,
                volume: request.Volume,
                executionPrice: latestTick.Price, // Precio tomado de la caché
                timestamp: DateTimeOffset.UtcNow
            );

            // 3. Persistir en base de datos
            await _tradeRepository.AddAsync(trade, cancellationToken);

            return trade.Id;
        }
    }
}