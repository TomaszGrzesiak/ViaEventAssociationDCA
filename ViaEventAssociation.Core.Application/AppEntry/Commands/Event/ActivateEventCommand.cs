using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry.Commands.Event;

public class ActivateEventCommand
{
    public EventId EventId { get; }

    private ActivateEventCommand(EventId eventId)
    {
        EventId = eventId;
    }

    public static Result<ActivateEventCommand> Create(string guidString)
    {
        var result = EventId.FromString(guidString);
        if (result.IsFailure) return Result<ActivateEventCommand>.Failure(result.Errors);


        var command = new ActivateEventCommand(result.Payload!);
        return Result<ActivateEventCommand>.Success(command);
    }
}