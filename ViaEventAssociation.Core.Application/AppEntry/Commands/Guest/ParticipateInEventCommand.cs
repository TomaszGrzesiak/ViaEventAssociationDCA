using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry.Commands.Guest;

public class ParticipateInEventCommand
{
    public EventId EventId { get; }
    public GuestId GuestId { get; }

    private ParticipateInEventCommand(EventId eventId, GuestId guestId)
    {
        EventId = eventId;
        GuestId = guestId;
    }

    public static Result<ParticipateInEventCommand> Create(string eventGuidString, string guestGuidString)
    {
        Error[] errors = [];

        // try to create EventId
        var resultEventId = EventId.FromString(eventGuidString);
        if (resultEventId.IsFailure)
            errors = [..errors, ..resultEventId.Errors];

        // try to create GuestId
        var resultGuestId = GuestId.FromString(guestGuidString);
        if (resultGuestId.IsFailure)
            errors = [..errors, ..resultGuestId.Errors];

        // if any errors - return them
        if (errors.Length > 0) return Result<ParticipateInEventCommand>.Failure(errors);

        // if no errors, finish the rest
        var eventId = resultEventId.Payload!;
        var guestId = resultGuestId.Payload!;
        var command = new ParticipateInEventCommand(eventId, guestId);

        return Result<ParticipateInEventCommand>.Success(command);
    }
}