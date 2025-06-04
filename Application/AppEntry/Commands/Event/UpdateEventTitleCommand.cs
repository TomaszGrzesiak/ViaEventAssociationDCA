using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace Application.AppEntry.Commands.Event;

public class UpdateEventTitleCommand
{
    public EventId EventId { get; set; }
    public string Title { get; set; }

    public UpdateEventTitleCommand(EventId eventId, string title)
    {
        EventId = eventId;
        Title = title;
    }

    public static Result<UpdateEventTitleCommand> Create(EventId eventId, string title)
    {
        var command = new UpdateEventTitleCommand(eventId, title);

        return Result<UpdateEventTitleCommand>.Success(command);
    }
}