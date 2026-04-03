using Mini_ETRM.Domain.Entities;
using Mini_ETRM.Domain.Enums;
using Mini_ETRM.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace Mini_ETRM.Infrastructure.Services
{
    public class MarketDataSimulatorService : BackgroundService
    {
        private readonly IMarketDataCache _cache;
        private readonly ILogger<MarketDataSimulatorService> _logger;
        private readonly Random _random = new();

        // Prices are initialized at a reasonable level for WTI and Brent.
        // In a real scenario, these could come from an external source or be set to more dynamic initial values.
        private decimal _currentWtiPrice = 75.00m;
        private decimal _currentBrentPrice = 80.00m;

        public MarketDataSimulatorService(IMarketDataCache cache, ILogger<MarketDataSimulatorService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Market Data Simulator is starting.");

            // Initialize the cache with the starting prices before we start fluctuating
            UpdatePrices();

            // Every 500ms we update the prices and store them in the cache
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(500, stoppingToken);
                UpdatePrices();
            }
        }

        private void UpdatePrices()
        {
            // The price changes between -0.25 and +0.25 cents per tick
            decimal wtiChange = (decimal)(_random.NextDouble() * 0.5 - 0.25);
            decimal brentChange = (decimal)(_random.NextDouble() * 0.5 - 0.25);

            _currentWtiPrice = Math.Round(_currentWtiPrice + wtiChange, 2);
            _currentBrentPrice = Math.Round(_currentBrentPrice + brentChange, 2);

            var wtiTick = new MarketTick(Commodity.WTI, _currentWtiPrice, DateTimeOffset.UtcNow);
            var brentTick = new MarketTick(Commodity.Brent, _currentBrentPrice, DateTimeOffset.UtcNow);

            _cache.UpdateTick(wtiTick);
            _cache.UpdateTick(brentTick);

            // Uncomment to see the spam in console, ideal for debugging.
            //_logger.LogInformation($"New Tick -> WTI: {_currentWtiPrice} | Brent: {_currentBrentPrice}");
        }
    }
}