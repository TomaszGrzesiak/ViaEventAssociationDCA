using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Events;

public sealed class EventTimeRange : ValueObject
{
    public DateTime? StartTime { get; }
    public DateTime? EndTime { get; }

    private EventTimeRange(DateTime? startTime, DateTime? endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }

    public static Result<EventTimeRange> Create(DateTime? startTime, DateTime? endTime)
    {
        var instance = new EventTimeRange(startTime, endTime);
        var validation = instance.Validate();
        return validation.IsSuccess
            ? Result<EventTimeRange>.Success(instance)
            : Result<EventTimeRange>.Failure(validation.Errors.ToArray());
    }

    public Result Validate()
    {
        var errors = new List<Error>();

        if (StartTime is null || EndTime is null)
        {
            return Result.Failure(Error.EventTimeRangeMissing);
        }

        if (StartTime >= EndTime)
            errors.Add(Error.EventTimeStartAfterEndTime);

        var duration = EndTime.Value - StartTime.Value;

        if (duration.TotalMinutes < 60)
            errors.Add(Error.EventTimeDurationTooShort);
        if (duration.TotalHours > 10)
            errors.Add(Error.EventTimeDurationTooLong);

        var start = StartTime.Value;
        var end = EndTime.Value;

        var validStart = start.TimeOfDay >= TimeSpan.FromHours(8); // TimeOfDay returns a timespan from midnight. Basically it checks if the time is after 8am.
        var validEndSameDay = end.Date == start.Date && end.TimeOfDay <= TimeSpan.FromHours(23.99);
        var validEndNextDay = end.Date == start.Date.AddDays(1) && end.TimeOfDay <= TimeSpan.FromHours(1);

        if (!(validEndSameDay || validEndNextDay))
            errors.Add(Error.EventTimeInvalidEndTimeWindow);

        if (!validStart)
            errors.Add(Error.EventTimeInvalidStartTime);

        return errors.Count == 0 ? Result.Success() : Result.Failure(errors.ToArray());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return StartTime;
        yield return EndTime;
    }

    public override string ToString() =>
        StartTime.HasValue && EndTime.HasValue
            ? $"{StartTime:yyyy-MM-dd HH:mm} to {EndTime:yyyy-MM-dd HH:mm}"
            : "Invalid Time Range";
}