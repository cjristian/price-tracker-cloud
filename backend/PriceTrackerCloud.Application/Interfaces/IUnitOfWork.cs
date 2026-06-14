using PriceTrackerCloud.Application.Interfaces.Repositories;

namespace PriceTrackerCloud.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IProductRepository Products { get; }
    IPriceRepository Prices { get; }
    IAlertRepository Alerts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
