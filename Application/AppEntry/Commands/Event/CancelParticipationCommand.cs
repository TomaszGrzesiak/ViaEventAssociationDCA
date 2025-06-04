using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace Application.AppEntry.Commands.Event;

public class CancelParticipationCommand
{
    public EventId EventId { get; set; }
    public GuestId GuestId { get; set; }

    public CancelParticipationCommand(EventId eventId, GuestId guestId)
    {
        EventId = eventId;
        GuestId = guestId;
    }

    public static Result<CancelParticipationCommand> Create(int id, int guestId)
    {
        var command = new CancelParticipationCommand(new EventId(new Guid()), new GuestId(new Guid()));

        return Result<CancelParticipationCommand>.Success(command);
    }
}