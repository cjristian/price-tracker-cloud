using MediatR;
using PriceTrackerCloud.Application.DTOs.Alerts;

namespace PriceTrackerCloud.Application.Queries.Alerts;

public record GetUserAlertsQuery(Guid UserId) : IRequest<IEnumerable<AlertDto>>;
