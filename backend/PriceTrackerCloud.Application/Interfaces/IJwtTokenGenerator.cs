using PriceTrackerCloud.Domain.Entities;

namespace PriceTrackerCloud.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
