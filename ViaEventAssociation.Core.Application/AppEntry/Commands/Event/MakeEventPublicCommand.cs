using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry.Commands.Event;

public class MakeEventPublicCommand
{
    public EventId EventId { get; }

    private MakeEventPublicCommand(EventId eventId)
    {
        EventId = eventId;
    }

    public static Result<MakeEventPublicCommand> Create(string guid)
    {
        var resultEventId = EventId.FromString(guid);
        if (resultEventId.IsFailure) return Result<MakeEventPublicCommand>.Failure(resultEventId.Errors.ToArray());

        var eventId = resultEventId.Payload!;

        var command = new MakeEventPublicCommand(eventId);
        return Result<MakeEventPublicCommand>.Success(command);
    }
}