using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry.Commands.Event;

public class UpdateEventTimeRangeCommand
{
    public EventId EventId { get; set; }
    public EventTimeRange EventTimeRange { get; set; }

    public UpdateEventTimeRangeCommand(EventId eventId, EventTimeRange eventTimeRange)
    {
        EventId = eventId;
        EventTimeRange = eventTimeRange;
    }

    public static Result<UpdateEventTimeRangeCommand> Create(string guid, string? startTime, string? endTime)
    {
        var errors = new List<Error>();

        var resultEventId = EventId.FromString(guid);
        if (resultEventId.IsFailure) errors.AddRange(resultEventId.Errors);

        var eventId = resultEventId.Payload!;

        // try to parse the string times. Returns Failure if not parsable, with an optional error message about wrong guid. 
        // if everything goes well, then declares two new variables: startDateTimeFormat and endDateTimeFormat.
        if (!DateTime.TryParse(startTime, out var startDateTimeFormat))
            return Result<UpdateEventTimeRangeCommand>.Failure([Error.TimeNotParsable, ..errors.ToArray()]);
        if (!DateTime.TryParse(endTime, out var endDateTimeFormat))
            return Result<UpdateEventTimeRangeCommand>.Failure([Error.TimeNotParsable, ..errors.ToArray()]);

        var resultEventTimeRange = EventTimeRange.Create(startDateTimeFormat, endDateTimeFormat);
        if (resultEventTimeRange.IsFailure) errors.AddRange(resultEventTimeRange.Errors);

        if (errors.Count > 0) return Result<UpdateEventTimeRangeCommand>.Failure(errors.ToArray());

        var newEventTimeRange = resultEventTimeRange.Payload!;

        var command = new UpdateEventTimeRangeCommand(eventId, newEventTimeRange);
        return Result<UpdateEventTimeRangeCommand>.Success(command);
    }
}