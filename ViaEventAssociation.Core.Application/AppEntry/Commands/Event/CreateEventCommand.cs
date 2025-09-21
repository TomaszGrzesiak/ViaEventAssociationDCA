using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry.Commands.Event;

public class CreateEventCommand
{
    public readonly EventId EventId;

    private CreateEventCommand(EventId eventId)
    {
        EventId = eventId;
    }

    public static Result<CreateEventCommand> Create(string guidString)
    {
        var result = EventId.FromString(guidString);
        if (result.IsFailure) return Result<CreateEventCommand>.Failure(result.Errors.ToArray());

        var eventId = result.Payload!;
        return Result<CreateEventCommand>.Success(new CreateEventCommand(eventId));
    }
}