using Mini_ETRM.Domain.Entities;
using Mini_ETRM.Domain.Enums;

namespace Mini_ETRM.Domain.Interfaces
{
    public interface IMarketDataCache
    {
        /// <summary>
        /// Guarda el último tick del simulador. Thread-safe.
        /// </summary>
        void UpdateTick(MarketTick tick);

        /// <summary>
        /// Obtiene el último tick en O(1) para cruzarlo con el Trade.
        /// </summary>
        MarketTick? GetLatestTick(Commodity commodity);
    }
}