using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry.Commands.Event;

public class UpdateEventTitleCommand
{
    public EventId EventId { get; }
    public EventTitle NewTitle { get; }

    private UpdateEventTitleCommand(EventId eventId, EventTitle newTitle)
    {
        EventId = eventId;
        NewTitle = newTitle;
    }

    public static Result<UpdateEventTitleCommand> Create(string guidString, string title)
    {
        Error[] errors = [];

        // try to create EventId
        var resultEventId = EventId.FromString(guidString);
        if (resultEventId.IsFailure)
            errors = [..errors, ..resultEventId.Errors];

        // try to create EventTitle
        var resultEventTitle = EventTitle.Create(title);
        if (resultEventTitle.IsFailure)
            errors = [..errors, ..resultEventTitle.Errors];

        // if any errors - return them
        if (errors.Length > 0) return Result<UpdateEventTitleCommand>.Failure(errors);

        // if no errors, finish the rest
        var newTitle = resultEventTitle.Payload!;
        var eventId = resultEventId.Payload!;
        var command = new UpdateEventTitleCommand(eventId, newTitle);

        return Result<UpdateEventTitleCommand>.Success(command);
    }
}