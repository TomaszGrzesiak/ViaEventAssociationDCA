using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry.Commands.Event;

public class UpdateEventDescriptionCommand
{
    public EventId EventId { get; set; }
    public EventDescription EventDescription { get; set; }

    public UpdateEventDescriptionCommand(EventId eventId, EventDescription eventDescription)
    {
        EventId = eventId;
        EventDescription = eventDescription;
    }

    public static Result<UpdateEventDescriptionCommand> Create(string guid, string title)
    {
        Error[] errors = [];

        var resultEventId = EventId.FromString(guid);
        if (resultEventId.IsFailure)
            errors = [..errors, ..resultEventId.Errors];

        var eventId = resultEventId.Payload!;

        var resultEventDescription = EventDescription.Create(title);
        if (resultEventDescription.IsFailure)
            errors = [..errors, ..resultEventDescription.Errors];

        var newDescription = resultEventDescription.Payload!;
        if (errors.Length > 0) return Result<UpdateEventDescriptionCommand>.Failure(errors);

        var cmd = new UpdateEventDescriptionCommand(eventId, newDescription);
        return Result<UpdateEventDescriptionCommand>.Success(cmd);
    }
}