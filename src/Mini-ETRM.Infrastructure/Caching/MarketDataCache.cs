using System.Collections.Concurrent;
using Mini_ETRM.Domain.Entities;
using Mini_ETRM.Domain.Enums;
using Mini_ETRM.Domain.Interfaces;

namespace Mini_ETRM.Infrastructure.Cahing
{
    public class MarketDataCache : IMarketDataCache
    {
        // ConcurrentDictionary maneja los bloqueos (locks) internamente por nosotros
        private readonly ConcurrentDictionary<Commodity, MarketTick> _latestTicks = new();

        public void UpdateTick(MarketTick tick)
        {
            // AddOrUpdate asegura que siempre tengamos el último valor sin race conditions
            _latestTicks.AddOrUpdate(tick.Commodity, tick, (_, _) => tick);
        }

        public MarketTick? GetLatestTick(Commodity commodity)
        {
            _latestTicks.TryGetValue(commodity, out var tick);
            return tick;
        }
    }

}