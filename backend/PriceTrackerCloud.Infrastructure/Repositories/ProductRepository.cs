using Microsoft.EntityFrameworkCore;
using PriceTrackerCloud.Application.Interfaces.Repositories;
using PriceTrackerCloud.Domain.Entities;
using PriceTrackerCloud.Infrastructure.Data;

namespace PriceTrackerCloud.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(PriceTrackerDbContext context) : base(context) { }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category) =>
        await _dbSet.Where(p => p.Category == category).ToListAsync();
}
