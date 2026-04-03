using Mini_ETRM.Domain.Entities;
using Mini_ETRM.Domain.Enums;

namespace Mini_ETRM.Domain.Interfaces
{
    public interface IMarketDataCache
    {
        // Stores the latest tick from the simulator. Thread-safe.
        void UpdateTick(MarketTick tick);

        // Returns the latest tick to cross it with the Trade.
        MarketTick? GetLatestTick(Commodity commodity);
    }
}