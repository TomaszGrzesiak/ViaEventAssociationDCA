using Application.AppEntry;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Guest;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.Features.Guest;

public class CancelEventParticipationHandler(IEventRepository eventRepository, IGuestRepository guestRepository, IUnitOfWork unitOfWork, ISystemTime systemTime)
    : ICommandHandler<CancelEventParticipationCommand>
{
    public async Task<Result> HandleAsync(CancelEventParticipationCommand command)
    {
        var errors = new List<Error>();
        // Load
        var ev = await eventRepository.GetAsync(command.EventId);
        if (ev == null) errors.Add(Error.EventNotFound);

        var guest = await guestRepository.GetAsync(command.GuestId);
        if (guest == null) errors.Add(Error.GuestNotFound);

        if (errors.Any()) return Result.Failure(errors);

        // Business change
        var result = ev!.CancelParticipation(guest!.Id, systemTime);
        if (result.IsFailure) return result;

        // Save the change
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}