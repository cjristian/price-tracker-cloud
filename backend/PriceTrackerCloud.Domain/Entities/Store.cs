using PriceTrackerCloud.Domain.Common;

namespace PriceTrackerCloud.Domain.Entities;

public class Store : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;

    public ICollection<ProductPrice> Prices { get; set; } = new List<ProductPrice>();
}
