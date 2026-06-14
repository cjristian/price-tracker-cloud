using MediatR;

namespace PriceTrackerCloud.Application.Commands.Alerts;

public record DeleteAlertCommand(Guid AlertId, Guid UserId) : IRequest;
