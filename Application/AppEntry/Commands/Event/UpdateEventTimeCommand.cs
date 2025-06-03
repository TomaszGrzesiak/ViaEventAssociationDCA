using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace Application.AppEntry.Commands.Event;

public class UpdateEventTimeCommand
{
    public EventId Id { get; set; }
    public DateTime startTime { get; set; }
    public DateTime endTime { get; set; }

    public UpdateEventTimeCommand(EventId id, DateTime startTime, DateTime endTime)
    {
        Id = id;
        this.startTime = startTime;
        this.endTime = endTime;
    }

    public static Result<UpdateEventTimeCommand> Create(int id, DateTime startTime, DateTime endTime)
    {
        var command = new UpdateEventTimeCommand(new EventId(new Guid()), startTime, endTime);
        return Result<UpdateEventTimeCommand>.Success(command);
    }
}