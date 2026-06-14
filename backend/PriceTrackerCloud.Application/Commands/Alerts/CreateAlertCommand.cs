using MediatR;
using PriceTrackerCloud.Application.DTOs.Alerts;

namespace PriceTrackerCloud.Application.Commands.Alerts;

public record CreateAlertCommand(Guid UserId, Guid ProductId, decimal TargetPrice) : IRequest<AlertDto>;
