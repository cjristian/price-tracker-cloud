using PriceTrackerCloud.Domain.Entities;

namespace PriceTrackerCloud.Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsWithEmailAsync(string email);
}
