using System.Collections.Concurrent;
using Mini_ETRM.Domain.Entities;
using Mini_ETRM.Domain.Enums;
using Mini_ETRM.Domain.Interfaces;

namespace Mini_ETRM.Infrastructure.Cahing
{
    public class MarketDataCache : IMarketDataCache
    {
        // ConcurrentDictionary handles locking internally for us, ensuring thread safety
        private readonly ConcurrentDictionary<Commodity, MarketTick> _latestTicks = new();

        public void UpdateTick(MarketTick tick)
        {
            // AddOrUpdate ensures we always have the latest value without race conditions
            _latestTicks.AddOrUpdate(tick.Commodity, tick, (_, _) => tick);
        }

        public MarketTick? GetLatestTick(Commodity commodity)
        {
            _latestTicks.TryGetValue(commodity, out var tick);
            return tick;
        }
    }
}