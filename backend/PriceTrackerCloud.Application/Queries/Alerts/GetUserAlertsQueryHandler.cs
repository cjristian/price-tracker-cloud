using AutoMapper;
using MediatR;
using PriceTrackerCloud.Application.DTOs.Alerts;
using PriceTrackerCloud.Application.Interfaces;

namespace PriceTrackerCloud.Application.Queries.Alerts;

public class GetUserAlertsQueryHandler : IRequestHandler<GetUserAlertsQuery, IEnumerable<AlertDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetUserAlertsQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AlertDto>> Handle(GetUserAlertsQuery request, CancellationToken cancellationToken)
    {
        var alerts = await _uow.Alerts.GetByUserAsync(request.UserId);
        return _mapper.Map<IEnumerable<AlertDto>>(alerts);
    }
}
