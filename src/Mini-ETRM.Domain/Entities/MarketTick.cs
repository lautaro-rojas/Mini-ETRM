using Mini_ETRM.Domain.Enums;

namespace Mini_ETRM.Domain.Entities
{
    public record MarketTick(Commodity Commodity, decimal Price, DateTimeOffset Timestamp);
}

/*
A record in .NET is a reference type that allows you to define immutable objects.
Records are immutable, meaning their properties are set at creation and cannot be modified afterward. 
This is useful for modeling data that should not change.
*/