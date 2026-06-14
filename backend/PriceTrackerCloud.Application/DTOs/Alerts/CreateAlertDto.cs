namespace PriceTrackerCloud.Application.DTOs.Alerts;

public record CreateAlertDto(Guid ProductId, decimal TargetPrice);
