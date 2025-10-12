using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry.Commands.Event;

public class MakeEventPrivateCommand
{
    public EventId EventId { get; }

    private MakeEventPrivateCommand(EventId eventId)
    {
        EventId = eventId;
    }

    public static Result<MakeEventPrivateCommand> Create(string guidString)
    {
        var resultEventId = EventId.FromString(guidString);
        if (resultEventId.IsFailure) return Result<MakeEventPrivateCommand>.Failure(resultEventId.Errors.ToArray());

        var eventId = resultEventId.Payload!;

        var command = new MakeEventPrivateCommand(eventId);
        return Result<MakeEventPrivateCommand>.Success(command);
    }
}