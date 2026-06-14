namespace PriceTrackerCloud.Application.DTOs.Alerts;

public record AlertDto(Guid Id, Guid ProductId, string ProductName, decimal TargetPrice, bool IsActive, DateTime CreatedAt);
