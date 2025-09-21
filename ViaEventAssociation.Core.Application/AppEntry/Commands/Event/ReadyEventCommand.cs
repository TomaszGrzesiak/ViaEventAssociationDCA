using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry.Commands.Event;

public class ReadyEventCommand
{
    public EventId EventId { get; }

    private ReadyEventCommand(EventId eventId)
    {
        EventId = eventId;
    }

    public static Result<ReadyEventCommand> Create(string guidString)
    {
        var result = EventId.FromString(guidString);
        if (result.IsFailure) return Result<ReadyEventCommand>.Failure(result.Errors);

        var cmd = new ReadyEventCommand(result.Payload!);
        return Result<ReadyEventCommand>.Success(cmd);
    }
}