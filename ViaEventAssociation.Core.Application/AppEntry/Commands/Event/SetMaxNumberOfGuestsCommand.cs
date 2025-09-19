using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry.Commands.Event;

public class SetMaxNumberOfGuestsCommand
{
    public EventId EventId { get; }
    public MaxGuests MaxGuests { get; }

    private SetMaxNumberOfGuestsCommand(EventId eventId, MaxGuests maxGuests)
    {
        EventId = eventId;
        MaxGuests = maxGuests;
    }

    public static Result<SetMaxNumberOfGuestsCommand> Create(string guid, int maxGuestsNo)
    {
        Error[] errors = [];

        // try to create EventId
        var resultEventId = EventId.FromString(guid);
        if (resultEventId.IsFailure)
            errors = [..errors, ..resultEventId.Errors];

        // try to create MaxGuest
        var resultMaxGuests = MaxGuests.Create(maxGuestsNo);
        if (resultMaxGuests.IsFailure)
            errors = [..errors, ..resultMaxGuests.Errors];

        // if any errors - return them
        if (errors.Length > 0) return Result<SetMaxNumberOfGuestsCommand>.Failure(errors);

        // if no errors, finish the rest
        var eventId = resultEventId.Payload!;
        var maxGuests = resultMaxGuests.Payload!;
        var cmd = new SetMaxNumberOfGuestsCommand(eventId, maxGuests);

        return Result<SetMaxNumberOfGuestsCommand>.Success(cmd);
    }
}