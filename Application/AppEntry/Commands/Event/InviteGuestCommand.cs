using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace Application.AppEntry.Commands.Event;

public class InviteGuestCommand
{
    public EventId EventId { get; set; }
    public GuestId GuestId { get; set; }

    public InviteGuestCommand(EventId eventId, GuestId guestId)
    {
        EventId = eventId;
        GuestId = guestId;
    }

    public static Result<InviteGuestCommand> Create(EventId eventId, GuestId guestId)
    {
        var command = new InviteGuestCommand(eventId,guestId);
        
        return Result<InviteGuestCommand>.Success(command);
    }
}