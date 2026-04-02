using Mini_ETRM.Domain.Entities;
using Mini_ETRM.Domain.Interfaces;
using Mini_ETRM.Infrastructure.Data;

namespace Mini_ETRM.Infrastructure.Repositories
{
    public class TradeRepository : ITradeRepository
    {
        private readonly ApplicationDbContext _context;

        public TradeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Trade trade, CancellationToken cancellationToken = default)
        {
            await _context.Trades.AddAsync(trade, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Trade?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Trades.FindAsync(new object[] { id }, cancellationToken);
        }
    }

}