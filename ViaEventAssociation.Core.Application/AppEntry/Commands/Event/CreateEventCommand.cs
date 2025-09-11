using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry.Commands.Event;

public class CreateEventCommand
{
    public EventId _eventId;

    private CreateEventCommand(EventId eventId)
    {
        _eventId = eventId;
    }

    public static Result<CreateEventCommand> Create(string guid)
    {
        var result = EventId.FromString(guid);
        if (result.IsFailure) return Result<CreateEventCommand>.Failure(result.Errors.ToArray());

        var eventId = result.Payload!;
        return Result<CreateEventCommand>.Success(new CreateEventCommand(eventId));
    }
}