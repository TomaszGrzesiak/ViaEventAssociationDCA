using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace Application.AppEntry.Commands.Event;

public class CreateEventCommand
{
    private CreateEventCommand(EventId id)
    {
        Id = id;
    }

    public EventId Id { get; set; }

    public static Result<CreateEventCommand> Create(int id)
    {
        var command = new CreateEventCommand(new EventId(Guid.NewGuid()));
        return Result<CreateEventCommand>.Success(command);
    }
    
    
}