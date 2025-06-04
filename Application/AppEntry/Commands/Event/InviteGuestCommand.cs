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

    public static Result<InviteGuestCommand> Create(int evenId, int userId)
    {
        var command = new InviteGuestCommand(new EventId(new Guid()), new GuestId(new Guid()));
        return Result<InviteGuestCommand>.Success(command);
    }
}