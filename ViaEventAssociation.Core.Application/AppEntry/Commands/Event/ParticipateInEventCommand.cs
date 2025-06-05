using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry.Commands.Event;

public class ParticipateInEventCommand
{
    public EventId EventId { get; set; }
    public GuestId GuestId { get; set; }

    public ParticipateInEventCommand(EventId eventId, GuestId guestId)
    {
        EventId = eventId;
        GuestId = guestId;
    }

    public static Result<ParticipateInEventCommand> Create(EventId eventId, GuestId guestId)
    {
        var command = new ParticipateInEventCommand(eventId, guestId);
        
        return Result<ParticipateInEventCommand>.Success(command);
    }
}