using PriceTrackerCloud.Domain.Entities;

namespace PriceTrackerCloud.Application.Interfaces.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryAsync(string category);
}
