using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace Application.AppEntry.Commands.Event;

public class CreateEventCommand
{
    private CreateEventCommand(EventId eventId)
    {
        EventId = eventId;
    }

    public EventId EventId { get; set; }

    public static Result<CreateEventCommand> Create(EventId eventId)
    {
        var command = new CreateEventCommand(eventId);
        
        return Result<CreateEventCommand>.Success(command);
    }
    
}