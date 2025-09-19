using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry.Commands.Event;

public class UpdateEventDescriptionCommand
{
    public EventId EventId { get; }
    public EventDescription EventDescription { get; }

    private UpdateEventDescriptionCommand(EventId eventId, EventDescription eventDescription)
    {
        EventId = eventId;
        EventDescription = eventDescription;
    }

    public static Result<UpdateEventDescriptionCommand> Create(string guid, string title)
    {
        Error[] errors = [];

        // try to create EventId
        var resultEventId = EventId.FromString(guid);
        if (resultEventId.IsFailure)
            errors = [..errors, ..resultEventId.Errors];

        // try to create EventDescription
        var resultEventDescription = EventDescription.Create(title);
        if (resultEventDescription.IsFailure)
            errors = [..errors, ..resultEventDescription.Errors];

        // if any errors - return them
        if (errors.Length > 0) return Result<UpdateEventDescriptionCommand>.Failure(errors);

        // if no errors, finish the rest
        var newDescription = resultEventDescription.Payload!;
        var eventId = resultEventId.Payload!;
        var cmd = new UpdateEventDescriptionCommand(eventId, newDescription);

        return Result<UpdateEventDescriptionCommand>.Success(cmd);
    }
}