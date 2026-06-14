using PriceTrackerCloud.Domain.Common;
using PriceTrackerCloud.Domain.Enums;

namespace PriceTrackerCloud.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}
