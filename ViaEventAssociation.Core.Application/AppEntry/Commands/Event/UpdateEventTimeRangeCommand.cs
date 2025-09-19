using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry.Commands.Event;

public class UpdateEventTimeRangeCommand
{
    public EventId EventId { get; }
    public EventTimeRange EventTimeRange { get; }

    private UpdateEventTimeRangeCommand(EventId eventId, EventTimeRange eventTimeRange)
    {
        EventId = eventId;
        EventTimeRange = eventTimeRange;
    }

    public static Result<UpdateEventTimeRangeCommand> Create(string guid, string? startTime, string? endTime)
    {
        var errors = new List<Error>();

        // try to create EventId
        var resultEventId = EventId.FromString(guid);
        if (resultEventId.IsFailure) errors.AddRange(resultEventId.Errors);

        // try to create EventTimeRange
        // try to parse the string times. Returns Failure if not parsable, with an optional error message about wrong guid. 
        // if everything goes well, then declares two new variables: startDateTimeFormat and endDateTimeFormat.
        if (!DateTime.TryParse(startTime, out var startDateTimeFormat))
            return Result<UpdateEventTimeRangeCommand>.Failure([Error.TimeNotParsable, ..errors.ToArray()]);
        if (!DateTime.TryParse(endTime, out var endDateTimeFormat))
            return Result<UpdateEventTimeRangeCommand>.Failure([Error.TimeNotParsable, ..errors.ToArray()]);

        var resultEventTimeRange = EventTimeRange.Create(startDateTimeFormat, endDateTimeFormat);
        if (resultEventTimeRange.IsFailure) errors.AddRange(resultEventTimeRange.Errors);

        // if any errors - return them
        if (errors.Count > 0) return Result<UpdateEventTimeRangeCommand>.Failure(errors.ToArray());

        // if no errors, finish the rest
        var eventId = resultEventId.Payload!;
        var newEventTimeRange = resultEventTimeRange.Payload!;
        var command = new UpdateEventTimeRangeCommand(eventId, newEventTimeRange);

        return Result<UpdateEventTimeRangeCommand>.Success(command);
    }
}