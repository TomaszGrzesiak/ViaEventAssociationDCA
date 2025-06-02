using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Domain.Aggregates.Events;

public class EventTimeRangeTests
{
    private readonly DateTime _start = new DateTime(2026, 6, 1, 22, 0, 0);
    private readonly DateTime _end = new DateTime(2026, 6, 2, 0, 30, 0);

    [Fact]
    public void Create_WithValidSameDayTime_ReturnsSuccess()
    {
        var result = EventTimeRange.Create(_start, _end);

        Assert.True(result.IsSuccess);
        Assert.Equal(_start, result.Payload!.StartTime);
        Assert.Equal(_end, result.Payload.EndTime);
    }

    [Fact]
    public void Create_WithValidCrossDayTime_ReturnsSuccess()
    {
        var result = EventTimeRange.Create(_start, _end);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_WithNullStartOrEnd_ReturnsFailure_EventTimeRangeMissing()
    {
        var result1 = EventTimeRange.Create(null, DateTime.Now);
        var result2 = EventTimeRange.Create(DateTime.Now, null);

        Assert.True(result1.IsFailure);
        Assert.Single(result1.Errors);
        Assert.Equal(Error.EventTimeRangeMissing, result1.Errors[0]);

        Assert.True(result2.IsFailure);
        Assert.Single(result2.Errors);
        Assert.Equal(Error.EventTimeRangeMissing, result2.Errors[0]);
    }

    [Fact]
    public void Create_WhenStartDateTimeAfterEndDateTime_ReturnsFailure()
    {
        var result = EventTimeRange.Create(_start.AddDays(2), _end);

        Assert.True(result.IsFailure);
        Assert.Contains(Error.EventTimeStartDateAfterEndDate, result.Errors);
    }

    [Fact]
    public void Create_WhenDurationTooShort_ReturnsFailure()
    {
        var result = EventTimeRange.Create(_start, _start.AddMinutes(30));

        Assert.True(result.IsFailure);
        Assert.Contains(Error.EventTimeDurationTooShort, result.Errors);
    }

    [Fact]
    public void Create_WhenDurationTooLong_ReturnsFailure()
    {
        var result = EventTimeRange.Create(_start, _end.AddHours(10));

        Assert.True(result.IsFailure);
        Assert.Contains(Error.EventTimeDurationTooLong, result.Errors);
    }

    [Fact]
    public void Create_WhenStartTimeTooEarly_ReturnsFailure()
    {
        var start = new DateTime(2026, 6, 2, 2, 0, 0);
        var result = EventTimeRange.Create(start, _end);

        Assert.True(result.IsFailure);
        Assert.Contains(Error.EventTimeStartMustBeAfter8Am, result.Errors);
    }

    [Fact]
    public void Create_WhenEndTimeInvalid_ReturnsFailure()
    {
        var result = EventTimeRange.Create(_start, _end.AddHours(2));

        Assert.True(result.IsFailure);
        Assert.Contains(Error.EventTimeInvalidEndTimeWindow, result.Errors);
    }

    [Fact]
    public void Equality_WorksForIdenticalStartAndEnd()
    {
        var r1 = EventTimeRange.Create(_start, _end).Payload;
        var r2 = EventTimeRange.Create(_start, _end).Payload;

        Assert.Equal(r1, r2);
    }
}