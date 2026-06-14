using PriceTrackerCloud.Domain.Common;

namespace PriceTrackerCloud.Domain.Entities;

public class ProductPrice : BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid StoreId { get; set; }
    public decimal Price { get; set; }
    public DateTime DateCollected { get; set; } = DateTime.UtcNow;

    public Product Product { get; set; } = null!;
    public Store Store { get; set; } = null!;
}
