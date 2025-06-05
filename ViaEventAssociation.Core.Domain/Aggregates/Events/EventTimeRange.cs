using System.Diagnostics.CodeAnalysis;
using ViaEventAssociation.Core.Domain.Common.Bases;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Events;

public class EventTimeRange : ValueObject
{
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }

    private EventTimeRange(DateTime startTime, DateTime endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }

    public static Result<EventTimeRange> Create(DateTime? startTime, DateTime? endTime)
    {
        // first phase of validation
        if (startTime is null || endTime is null) return Result<EventTimeRange>.Failure(Error.EventTimeRangeMissing);

        // can't pass just startTime or endTime, because from a compiler perspective,
        // they are nullable. I know it's weird. We've just made sure they aren't null in the line above.
        var instance = new EventTimeRange(startTime.Value, endTime.Value);
        return Result<EventTimeRange>.Success(instance);
    }

    public static Result<EventTimeRange> Validate(EventTimeRange timeRange)
    {
        var errors = new List<Error>();

        if (timeRange.StartTime.Date > timeRange.EndTime.Date)
            errors.Add(Error.EventTimeStartDateAfterEndDate);

        if (timeRange.StartTime.Date == timeRange.EndTime.Date && timeRange.StartTime >= timeRange.EndTime)
            errors.Add(Error.EventTimeStartAfterEndTime);

        var duration = timeRange.EndTime - timeRange.StartTime;

        if (duration.TotalMinutes < 60)
            errors.Add(Error.EventTimeDurationTooShort);

        if (duration.TotalHours > 10)
            errors.Add(Error.EventTimeDurationTooLong);

        if (timeRange.StartTime < DateTime.Now)
            errors.Add(Error.EventTimeCannotStartInPast);

        var validStart = timeRange.StartTime.TimeOfDay >= TimeSpan.FromHours(8);

        var validEndSameDay =
            timeRange.EndTime.Date == timeRange.StartTime.Date &&
            timeRange.EndTime.TimeOfDay <= TimeSpan.FromHours(23.99);

        var validEndNextDay =
            timeRange.EndTime.Date == timeRange.StartTime.Date.AddDays(1) &&
            timeRange.EndTime.TimeOfDay <= TimeSpan.FromHours(1);

        if (!(validEndSameDay || validEndNextDay))
            errors.Add(Error.EventTimeInvalidEndTimeWindow);

        if (!validStart)
            errors.Add(Error.EventTimeStartMustBeAfter8Am);

        // Spans 01:00 to 08:00 window?
        var forbiddenWindowStart = timeRange.EndTime.Date.AddHours(1); // 01:00
        var forbiddenWindowEnd = timeRange.EndTime.Date.AddHours(8); // 08:00

        bool overlapsSleepTime = timeRange.StartTime < forbiddenWindowEnd && timeRange.EndTime > forbiddenWindowStart;
        if (overlapsSleepTime)
            errors.Add(Error.EventTimeCannotSpan01To08);

        return errors.Count == 0
            ? Result<EventTimeRange>.Success(timeRange)
            : Result<EventTimeRange>.Failure(errors.ToArray());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartTime;
        yield return EndTime;
    }

    public override string ToString() =>
        $"{StartTime:yyyy-MM-dd HH:mm} to {EndTime:yyyy-MM-dd HH:mm}";

    public static EventTimeRange Default()
    {
        var start = DateTime.Today.AddDays(1).AddHours(8);
        return new EventTimeRange(start, start.AddHours(3));
    }

    public static EventTimeRange ValidNonDefault()
    {
        var start = new DateTime(2077, 6, 30, 8, 0, 0);
        return new EventTimeRange(start, start.AddHours(3));
    }
}