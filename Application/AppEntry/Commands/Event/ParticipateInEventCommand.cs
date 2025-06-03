using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace Application.AppEntry.Commands.Event;

public class ParticipateInEventCommand
{
    public EventId EventId { get; set; }
    public GuestId GuestId { get; set; }

    public ParticipateInEventCommand(EventId eventId, GuestId guestId)
    {
        EventId = eventId;
        GuestId = guestId;
    }

    public static Result<ParticipateInEventCommand> Create(int id, int userId)
    {
        var command = new ParticipateInEventCommand(new EventId(new Guid()), new GuestId(new Guid()));
        return Result<ParticipateInEventCommand>.Success(command);
    }
}