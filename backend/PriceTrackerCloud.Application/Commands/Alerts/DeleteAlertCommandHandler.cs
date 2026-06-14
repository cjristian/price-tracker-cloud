using MediatR;
using PriceTrackerCloud.Application.Interfaces;

namespace PriceTrackerCloud.Application.Commands.Alerts;

public class DeleteAlertCommandHandler : IRequestHandler<DeleteAlertCommand>
{
    private readonly IUnitOfWork _uow;

    public DeleteAlertCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task Handle(DeleteAlertCommand request, CancellationToken cancellationToken)
    {
        var alert = await _uow.Alerts.GetByIdAsync(request.AlertId)
            ?? throw new KeyNotFoundException($"Alerta '{request.AlertId}' no encontrada.");

        // Solo el dueño puede borrar su alerta
        if (alert.UserId != request.UserId)
            throw new UnauthorizedAccessException("No tienes permiso para eliminar esta alerta.");

        _uow.Alerts.Remove(alert);
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
