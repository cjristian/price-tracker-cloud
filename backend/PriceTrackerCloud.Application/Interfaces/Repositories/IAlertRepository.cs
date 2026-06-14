using PriceTrackerCloud.Domain.Entities;

namespace PriceTrackerCloud.Application.Interfaces.Repositories;

public interface IAlertRepository : IRepository<Alert>
{
    Task<IEnumerable<Alert>> GetByUserAsync(Guid userId);
    Task<IEnumerable<Alert>> GetActiveAlertsAsync();
}
