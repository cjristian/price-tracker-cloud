using Microsoft.EntityFrameworkCore;
using PriceTrackerCloud.Application.Interfaces.Repositories;
using PriceTrackerCloud.Domain.Entities;
using PriceTrackerCloud.Infrastructure.Data;

namespace PriceTrackerCloud.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(PriceTrackerDbContext context) : base(context) { }

    public async Task<User?> GetByEmailAsync(string email) =>
        await _dbSet.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<bool> ExistsWithEmailAsync(string email) =>
        await _dbSet.AnyAsync(u => u.Email == email);
}
