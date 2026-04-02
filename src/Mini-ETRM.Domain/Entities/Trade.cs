using Mini_ETRM.Domain.Enums;

namespace Mini_ETRM.Domain.Entities
{
    public class Trade
    {
        public Guid Id { get; private set; }
        public Commodity Commodity { get; private set; }
        public TradeType Type { get; private set; }
        public decimal Volume { get; private set; }
        public decimal ExecutionPrice { get; private set; }
        public DateTimeOffset Timestamp { get; private set; }
        
        // Constructor without parameters for EF Core
        protected Trade() { }

        public Trade(Guid id, Commodity commodity, TradeType type, decimal volume, decimal executionPrice, DateTimeOffset timestamp)
        {
            if (volume <= 0) throw new ArgumentException("El volumen debe ser mayor a cero.", nameof(volume));
            if (executionPrice <= 0) throw new ArgumentException("El precio de ejecución debe ser mayor a cero.", nameof(executionPrice));

            Id = id;
            Commodity = commodity;
            Type = type;
            Volume = volume;
            ExecutionPrice = executionPrice;
            Timestamp = timestamp;
        }
    }
}