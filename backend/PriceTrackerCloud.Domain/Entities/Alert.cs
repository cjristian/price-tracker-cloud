using PriceTrackerCloud.Domain.Common;

namespace PriceTrackerCloud.Domain.Entities;

public class Alert : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public decimal TargetPrice { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
