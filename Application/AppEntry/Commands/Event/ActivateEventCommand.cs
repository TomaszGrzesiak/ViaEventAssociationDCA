using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace Application.AppEntry.Commands.Event;

public class ActivateEventCommand
{
    public EventId EventId { get; set; }

    public ActivateEventCommand(EventId eventId)
    {
        EventId = eventId;
    }

    public static Result<ActivateEventCommand> Create(EventId eventId)
    {
        var command = new ActivateEventCommand(eventId);

        return Result<ActivateEventCommand>.Success(command);
    }
}