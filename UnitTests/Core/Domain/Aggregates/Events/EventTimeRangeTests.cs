using System;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;
using Xunit;

public class EventTimeRangeTests
{
    [Fact]
    public void Create_WithValidSameDayTime_ReturnsSuccess()
    {
        var start = new DateTime(2025, 6, 1, 10, 0, 0);
        var end = new DateTime(2025, 6, 1, 18, 0, 0);

        var result = EventTimeRange.Create(start, end);

        Assert.True(result.IsSuccess);
        Assert.Equal(start, result.Payload.StartTime);
        Assert.Equal(end, result.Payload.EndTime);
    }

    [Fact]
    public void Create_WithValidCrossDayTime_ReturnsSuccess()
    {
        var start = new DateTime(2025, 6, 1, 22, 0, 0);
        var end = new DateTime(2025, 6, 2, 0, 30, 0);

        var result = EventTimeRange.Create(start, end);

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
    public void Create_WhenStartTimeAfterEndTime_ReturnsFailure()
    {
        var start = new DateTime(2025, 6, 1, 15, 0, 0);
        var end = new DateTime(2025, 6, 1, 10, 0, 0);

        var result = EventTimeRange.Create(start, end);

        Assert.True(result.IsFailure);
        Assert.Contains(Error.EventTimeStartAfterEndTime, result.Errors);
    }

    [Fact]
    public void Create_WhenDurationTooShort_ReturnsFailure()
    {
        var start = new DateTime(2025, 6, 1, 18, 0, 0);
        var end = new DateTime(2025, 6, 1, 18, 30, 0);

        var result = EventTimeRange.Create(start, end);

        Assert.True(result.IsFailure);
        Assert.Contains(Error.EventTimeDurationTooShort, result.Errors);
    }

    [Fact]
    public void Create_WhenDurationTooLong_ReturnsFailure()
    {
        var start = new DateTime(2025, 6, 1, 10, 0, 0);
        var end = new DateTime(2025, 6, 1, 21, 0, 0);

        var result = EventTimeRange.Create(start, end);

        Assert.True(result.IsFailure);
        Assert.Contains(Error.EventTimeDurationTooLong, result.Errors);
    }

    [Fact]
    public void Create_WhenStartTimeTooEarly_ReturnsFailure()
    {
        var start = new DateTime(2025, 6, 1, 7, 30, 0);
        var end = new DateTime(2025, 6, 1, 10, 0, 0);

        var result = EventTimeRange.Create(start, end);

        Assert.True(result.IsFailure);
        Assert.Contains(Error.EventTimeStartMustBeAfter8Am, result.Errors);
    }

    [Fact]
    public void Create_WhenEndTimeInvalid_ReturnsFailure()
    {
        var start = new DateTime(2025, 6, 1, 23, 30, 0);
        var end = new DateTime(2025, 6, 2, 2, 0, 0);

        var result = EventTimeRange.Create(start, end);

        Assert.True(result.IsFailure);
        Assert.Contains(Error.EventTimeInvalidEndTimeWindow, result.Errors);
    }

    [Fact]
    public void Equality_WorksForIdenticalStartAndEnd()
    {
        var start = new DateTime(2025, 6, 1, 10, 0, 0);
        var end = new DateTime(2025, 6, 1, 12, 0, 0);

        var r1 = EventTimeRange.Create(start, end).Payload;
        var r2 = EventTimeRange.Create(start, end).Payload;

        Assert.Equal(r1, r2);
    }
}