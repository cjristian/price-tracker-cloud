using PriceTrackerCloud.Domain.Entities;

namespace PriceTrackerCloud.Application.Interfaces.Repositories;

public interface IPriceRepository : IRepository<ProductPrice>
{
    Task<IEnumerable<ProductPrice>> GetHistoryByProductAsync(Guid productId);
    Task<ProductPrice?> GetLatestByProductAndStoreAsync(Guid productId, Guid storeId);
}
