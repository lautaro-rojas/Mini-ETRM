using Mini_ETRM.Domain.Entities;

namespace Mini_ETRM.Domain.Interfaces
{
    public interface ITradeRepository
    {
        Task AddAsync(Trade trade, CancellationToken cancellationToken = default);
        Task<Trade?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}