using UnitTests.Helpers;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Domain.Aggregates.Events.UseCasesTests;

public class EventTestsId4
{
    // ---------- SUCCESS SCENARIOS ----------

    [Theory]
    [InlineData("2025-08-25 19:00", "2025-08-25 23:59")]
    [InlineData("2025-08-25 12:00", "2025-08-25 16:30")]
    [InlineData("2025-08-25 08:00", "2025-08-25 12:15")]
    [InlineData("2025-08-25 10:00", "2025-08-25 20:00")]
    [InlineData("2025-08-25 13:00", "2025-08-25 23:00")]
    public void Id4_S1_ValidSameDayTimes_ShouldSucceed(string startStr, string endStr)
    {
        var ev = EventFactory.Init().WithStatus(EventStatus.Draft).Build();
        var range = EventTimeRange.Create(DateTime.Parse(startStr), DateTime.Parse(endStr)).Payload!;

        var result = ev.UpdateTimeRange(range);

        Assert.True(result.IsSuccess);
        Assert.Equal(range, ev.TimeRange);
    }

    [Theory]
    [InlineData("2025-08-25 19:00", "2025-08-26 01:00")]
    [InlineData("2025-08-25 12:00", "2025-08-25 16:30")]
    [InlineData("2025-08-25 08:00", "2025-08-25 12:15")]
    public void Id4_S2_EndNextDayBefore1am_ShouldSucceed(string startStr, string endStr)
    {
        var ev = EventFactory.Init().WithStatus(EventStatus.Draft).Build();
        var range = EventTimeRange.Create(DateTime.Parse(startStr), DateTime.Parse(endStr)).Payload!;

        var result = ev.UpdateTimeRange(range);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Id4_S3_UpdateWhenReady_ShouldResetStatusToDraft()
    {
        var ev = EventFactory.Init().WithStatus(EventStatus.Draft).Build();
        ev.UpdateMaxGuests(MaxGuests.Create(10));
        ev.UpdateVisibility(EventVisibility.Public);
        ev.Ready();

        var range = EventTimeRange.Create(DateTime.Parse("2025-08-25 12:00"), DateTime.Parse("2025-08-25 18:00")).Payload!;
        var result = ev.UpdateTimeRange(range);

        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Draft, ev.Status);
    }

    [Fact]
    public void Id4_S4_FutureStartTime_ShouldSucceed()
    {
        var futureStart = DateTime.Today.AddDays(1).AddHours(10);
        var futureEnd = futureStart.AddHours(4);

        var ev = EventFactory.Init().WithStatus(EventStatus.Draft).Build();
        var range = EventTimeRange.Create(futureStart, futureEnd).Payload!;

        var result = ev.UpdateTimeRange(range);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Id4_S5_DurationTenHours_ShouldSucceed()
    {
        var ev = EventFactory.Init().WithStatus(EventStatus.Draft).Build();
        var start = DateTime.Parse("2025-08-30 13:00");
        var end = start.AddHours(10);
        var range = EventTimeRange.Create(start, end).Payload!;

        var result = ev.UpdateTimeRange(range);

        Assert.True(result.IsSuccess);
    }

    // ---------- FAILURE SCENARIOS ----------

    [Theory]
    [InlineData("2025-08-26 19:00", "2025-08-25 01:00")]
    [InlineData("2025-08-26 19:00", "2025-08-25 23:59")]
    [InlineData("2025-08-27 12:00", "2025-08-25 16:30")]
    [InlineData("2025-08-01 08:00", "2025-07-31 12:15")]
    public void Id4_F1_StartAfterEndDate_ShouldFail(string startStr, string endStr)
    {
        var ev = EventFactory.Init().Build();
        var timeRange = EventTimeRange.Create(DateTime.Parse(startStr), DateTime.Parse(endStr));
        Assert.True(timeRange.IsSuccess);

        var result = ev.UpdateTimeRange(timeRange.Payload!);
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventTimeStartDateAfterEndDate);
    }

    [Theory]
    [InlineData("2025-08-26 19:00", "2025-08-26 14:00")]
    [InlineData("2025-08-26 16:00", "2025-08-26 00:00")]
    [InlineData("2025-08-26 19:00", "2025-08-26 18:59")]
    [InlineData("2025-08-26 12:00", "2025-08-26 10:10")]
    [InlineData("2025-08-26 08:00", "2025-08-26 00:30")]
    public void Id4_F2_StartTimeAfterEndTime_ShouldFail(string startStr, string endStr)
    {
        var ev = EventFactory.Init().Build();
        var timeRange = EventTimeRange.Create(DateTime.Parse(startStr), DateTime.Parse(endStr));
        Assert.True(timeRange.IsSuccess);

        var result = ev.UpdateTimeRange(timeRange.Payload!);
        Assert.True(result.IsFailure);

        Assert.Contains(result.Errors, e => e == Error.EventTimeStartAfterEndTime);
    }

    [Theory]
    [InlineData("2025-08-26 14:00", "2025-08-26 14:50")]
    [InlineData("2025-08-26 18:00", "2025-08-26 18:59")]
    [InlineData("2025-08-26 12:00", "2025-08-26 12:30")]
    [InlineData("2025-08-26 08:00", "2025-08-26 08:00")]
    public void Id4_F3_DurationTooShortSameDay_ShouldFail(string startStr, string endStr)
    {
        var ev = EventFactory.Init().Build();
        var timeRange = EventTimeRange.Create(DateTime.Parse(startStr), DateTime.Parse(endStr));
        Assert.True(timeRange.IsSuccess);

        var result = ev.UpdateTimeRange(timeRange.Payload!);
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventTimeDurationTooShort);
    }

    [Theory]
    [InlineData("2025-08-25 23:30", "2025-08-26 00:15")]
    [InlineData("2025-08-30 23:01", "2025-08-31 00:00")]
    [InlineData("2025-08-30 23:59", "2025-08-31 00:01")]
    public void Id4_F4_DurationTooShortCrossMidnight_ShouldFail(string startStr, string endStr)
    {
        var ev = EventFactory.Init().Build();
        var timeRange = EventTimeRange.Create(DateTime.Parse(startStr), DateTime.Parse(endStr));
        Assert.True(timeRange.IsSuccess);

        var result = ev.UpdateTimeRange(timeRange.Payload!);
        Assert.True(result.IsFailure);

        Assert.Contains(result.Errors, e => e == Error.EventTimeDurationTooShort);
    }

    [Theory]
    [InlineData("2025-08-25 07:50", "2025-08-25 14:00")]
    [InlineData("2025-08-25 07:59", "2025-08-25 15:00")]
    [InlineData("2025-08-25 01:01", "2025-08-25 08:30")]
    [InlineData("2025-08-25 05:59", "2025-08-25 07:59")]
    [InlineData("2025-08-25 00:59", "2025-08-25 07:59")]
    public void Id4_F5_StartTimeTooEarly_ShouldFail(string startStr, string endStr)
    {
        var ev = EventFactory.Init().Build();
        var timeRange = EventTimeRange.Create(DateTime.Parse(startStr), DateTime.Parse(endStr));
        Assert.True(timeRange.IsSuccess);

        var result = ev.UpdateTimeRange(timeRange.Payload!);
        Assert.True(result.IsFailure);

        Assert.Contains(result.Errors, e => e == Error.EventTimeStartMustBeAfter8Am);
    }

    [Theory]
    [InlineData("2025-08-24 23:50", "2025-08-25 01:01")]
    [InlineData("2025-08-24 22:00", "2025-08-25 07:59")]
    [InlineData("2025-08-30 23:00", "2025-08-31 02:30")]
    public void Id4_F6_EndTimeAfter1am_ShouldFail(string startStr, string endStr)
    {
        var ev = EventFactory.Init().Build();
        var timeRange = EventTimeRange.Create(DateTime.Parse(startStr), DateTime.Parse(endStr));
        Assert.True(timeRange.IsSuccess);

        var result = ev.UpdateTimeRange(timeRange.Payload!);
        Assert.True(result.IsFailure);

        Assert.Contains(result.Errors, e => e == Error.EventTimeInvalidEndTimeWindow);
    }

    [Fact]
    public void Id4_F7_UpdateTimeWhenActive_ShouldFail()
    {
        var ev = EventFactory.Init().WithStatus(EventStatus.Active).Build();

        var range = EventTimeRange.Create(DateTime.Parse("2025-08-30 13:00"), DateTime.Parse("2025-08-30 20:00")).Payload!;
        var result = ev.UpdateTimeRange(range);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.ActiveOrCanceledEventCannotBeModified);
    }

    [Fact]
    public void Id4_F8_UpdateTimeWhenCancelled_ShouldFail()
    {
        var ev = EventFactory.Init().WithStatus(EventStatus.Draft).Build();
        ev.Cancel();

        var range = EventTimeRange.Create(DateTime.Parse("2025-08-30 13:00"), DateTime.Parse("2025-08-30 20:00")).Payload!;
        var result = ev.UpdateTimeRange(range);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.ActiveOrCanceledEventCannotBeModified);
    }

    [Theory]
    [InlineData("2025-08-30 08:00", "2025-08-30 18:01")]
    [InlineData("2025-08-30 14:59", "2025-08-31 01:00")]
    [InlineData("2025-08-30 14:00", "2025-08-31 00:01")]
    [InlineData("2025-08-30 14:00", "2025-08-31 18:30")]
    public void Id4_F9_DurationTooLong_ShouldFail(string startStr, string endStr)
    {
        var ev = EventFactory.Init().Build();
        var timeRange = EventTimeRange.Create(DateTime.Parse(startStr), DateTime.Parse(endStr));
        Assert.True(timeRange.IsSuccess);

        var result = ev.UpdateTimeRange(timeRange.Payload!);
        Assert.True(result.IsFailure);

        Assert.Contains(result.Errors, e => e == Error.EventTimeDurationTooLong);
    }

    [Fact]
    public void Id4_F10_StartTimeInThePast_ShouldFail()
    {
        var pastStart = DateTime.Now.AddHours(-2);
        var pastEnd = pastStart.AddHours(2);

        var ev = EventFactory.Init().Build();
        var timeRange = EventTimeRange.Create(pastStart, pastEnd);
        Assert.True(timeRange.IsSuccess);

        var result = ev.UpdateTimeRange(timeRange.Payload!);
        Assert.True(result.IsFailure);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventTimeCannotStartInPast);
    }

    [Theory]
    [InlineData("2025-08-31 00:30", "2025-08-31 08:30")]
    [InlineData("2025-08-30 23:59", "2025-08-31 08:01")]
    [InlineData("2025-08-31 01:00", "2025-08-31 08:00")]
    public void Id4_F11_SpansInvalidTime_ShouldFail(string startStr, string endStr)
    {
        var ev = EventFactory.Init().Build();
        var timeRange = EventTimeRange.Create(DateTime.Parse(startStr), DateTime.Parse(endStr));
        Assert.True(timeRange.IsSuccess);

        var result = ev.UpdateTimeRange(timeRange.Payload!);
        Assert.True(result.IsFailure);

        Assert.Contains(result.Errors, e => e == Error.EventTimeCannotSpan01To08);
    }
}