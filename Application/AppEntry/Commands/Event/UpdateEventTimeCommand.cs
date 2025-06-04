using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace Application.AppEntry.Commands.Event;

public class UpdateEventTimeCommand
{
    public EventId EventId { get; set; }
    public DateTime startTime { get; set; }
    public DateTime endTime { get; set; }

    public UpdateEventTimeCommand(EventId eventId, DateTime startTime, DateTime endTime)
    {
        EventId = eventId;
        this.startTime = startTime;
        this.endTime = endTime;
    }

    public static Result<UpdateEventTimeCommand> Create(EventId eventId, DateTime startTime, DateTime endTime)
    {
        var command = new UpdateEventTimeCommand(eventId, startTime, endTime);
        
        return Result<UpdateEventTimeCommand>.Success(command);
    }
}