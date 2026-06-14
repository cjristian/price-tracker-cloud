using PriceTrackerCloud.Application.Interfaces;
using PriceTrackerCloud.Application.Interfaces.Repositories;
using PriceTrackerCloud.Infrastructure.Data;
using PriceTrackerCloud.Infrastructure.Repositories;

namespace PriceTrackerCloud.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly PriceTrackerDbContext _context;

    public IUserRepository Users { get; }
    public IProductRepository Products { get; }
    public IPriceRepository Prices { get; }
    public IAlertRepository Alerts { get; }

    public UnitOfWork(PriceTrackerDbContext context)
    {
        _context = context;
        Users    = new UserRepository(context);
        Products = new ProductRepository(context);
        Prices   = new PriceRepository(context);
        Alerts   = new AlertRepository(context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
