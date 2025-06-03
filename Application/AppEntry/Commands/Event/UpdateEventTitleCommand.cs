using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace Application.AppEntry.Commands.Event;

public class UpdateEventTitleCommand
{
    public EventId Id { get; set; }
    public string Title { get; set; }

    public UpdateEventTitleCommand(EventId id, string title)
    {
        Id = id;
        Title = title;
    }

    public static Result<UpdateEventTitleCommand> Create(int id, string title)
    {
        var command = new UpdateEventTitleCommand(new EventId(new Guid()), title);

        return Result<UpdateEventTitleCommand>.Success(command);
    }
}