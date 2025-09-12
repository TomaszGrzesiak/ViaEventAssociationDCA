using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry.Commands.Event;

public class UpdateEventTitleCommand
{
    public EventId EventId { get; set; }
    public EventTitle NewTitle { get; set; }

    public UpdateEventTitleCommand(EventId eventId, EventTitle newTitle)
    {
        EventId = eventId;
        NewTitle = newTitle;
    }

    public static Result<UpdateEventTitleCommand> Create(string guid, string title)
    {
        Error[] errors = [];

        var resultEventId = EventId.FromString(guid);
        if (resultEventId.IsFailure)
            errors = [..errors, ..resultEventId.Errors];

        var eventId = resultEventId.Payload!;

        var resultEventTitle = EventTitle.Create(title);
        if (resultEventTitle.IsFailure)
            errors = [..errors, ..resultEventTitle.Errors];

        var newTitle = resultEventTitle.Payload!;

        if (errors.Length > 0) return Result<UpdateEventTitleCommand>.Failure(errors);

        var command = new UpdateEventTitleCommand(eventId, newTitle);

        return Result<UpdateEventTitleCommand>.Success(command);
    }
}