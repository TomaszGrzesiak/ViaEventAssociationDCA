using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry.Commands.Guest;

public class CancelEventParticipationCommand
{
    public EventId EventId { get; }
    public GuestId GuestId { get; }

    private CancelEventParticipationCommand(EventId eventId, GuestId guestId)
    {
        EventId = eventId;
        GuestId = guestId;
    }

    public static Result<CancelEventParticipationCommand> Create(string eventGuidString, string guestGuidString)
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
        if (errors.Length > 0) return Result<CancelEventParticipationCommand>.Failure(errors);

        // if no errors, finish the rest
        var eventId = resultEventId.Payload!;
        var guestId = resultGuestId.Payload!;
        var command = new CancelEventParticipationCommand(eventId, guestId);

        return Result<CancelEventParticipationCommand>.Success(command);
    }
}