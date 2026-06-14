using PriceTrackerCloud.Domain.Common;

namespace PriceTrackerCloud.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    public ICollection<ProductPrice> Prices { get; set; } = new List<ProductPrice>();
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}
