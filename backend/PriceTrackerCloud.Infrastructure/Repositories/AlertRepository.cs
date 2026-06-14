using Microsoft.EntityFrameworkCore;
using PriceTrackerCloud.Application.Interfaces.Repositories;
using PriceTrackerCloud.Domain.Entities;
using PriceTrackerCloud.Infrastructure.Data;

namespace PriceTrackerCloud.Infrastructure.Repositories;

public class AlertRepository : Repository<Alert>, IAlertRepository
{
    public AlertRepository(PriceTrackerDbContext context) : base(context) { }

    public async Task<IEnumerable<Alert>> GetByUserAsync(Guid userId) =>
        await _dbSet
            .Include(a => a.Product)
            .Where(a => a.UserId == userId)
            .ToListAsync();

    public async Task<IEnumerable<Alert>> GetActiveAlertsAsync() =>
        await _dbSet
            .Include(a => a.Product)
            .Where(a => a.IsActive)
            .ToListAsync();
}
