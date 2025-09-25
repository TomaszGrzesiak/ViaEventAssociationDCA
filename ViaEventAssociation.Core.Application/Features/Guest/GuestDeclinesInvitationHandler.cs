using Application.AppEntry;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Guest;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.Features.Guest;

public class GuestDeclinesInvitationHandler(IEventRepository eventRepository, IGuestRepository guestRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<GuestDeclinesInvitationCommand>
{
    public async Task<Result> HandleAsync(GuestDeclinesInvitationCommand command)
    {
        // Load
        var ev = await eventRepository.GetAsync(command.EventId);
        if (ev == null)
            return Result.Failure(Error.EventNotFound);

        var guest = await guestRepository.GetAsync(command.GuestId);
        if (guest == null)
            return Result.Failure(Error.GuestNotFound);

        // Business change
        var result = ev.DeclineInvitation(command.GuestId);
        if (result.IsFailure) return result;

        // Save the change
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}