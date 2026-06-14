namespace PriceTrackerCloud.Application.DTOs.Prices;

public record ProductPriceDto(Guid Id, Guid ProductId, string StoreName, decimal Price, DateTime DateCollected);
