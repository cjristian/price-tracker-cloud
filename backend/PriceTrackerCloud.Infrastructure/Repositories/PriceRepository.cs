using Microsoft.EntityFrameworkCore;
using PriceTrackerCloud.Application.Interfaces.Repositories;
using PriceTrackerCloud.Domain.Entities;
using PriceTrackerCloud.Infrastructure.Data;

namespace PriceTrackerCloud.Infrastructure.Repositories;

public class PriceRepository : Repository<ProductPrice>, IPriceRepository
{
    public PriceRepository(PriceTrackerDbContext context) : base(context) { }

    public async Task<IEnumerable<ProductPrice>> GetHistoryByProductAsync(Guid productId) =>
        await _dbSet
            .Include(pp => pp.Store)
            .Where(pp => pp.ProductId == productId)
            .OrderBy(pp => pp.DateCollected)
            .ToListAsync();

    public async Task<ProductPrice?> GetLatestByProductAndStoreAsync(Guid productId, Guid storeId) =>
        await _dbSet
            .Where(pp => pp.ProductId == productId && pp.StoreId == storeId)
            .OrderByDescending(pp => pp.DateCollected)
            .FirstOrDefaultAsync();
}
