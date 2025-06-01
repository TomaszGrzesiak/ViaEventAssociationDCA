using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Domain.Aggregates.Events.UseCasesTests;

public class EventTestsId2
{
    [Theory]
    [InlineData("Scary Movie Night!")]
    [InlineData("Graduation Gala")]
    [InlineData("VIA Hackathon")]
    public void Id2_S1_TitleUpdate_WhenDraftOrReadyAndValidLength_ShouldSucceed(string exampleTitle)
    {
        var eventResult = VeaEvent.Create();
        Assert.True(eventResult.IsSuccess);
        var ev = eventResult.Payload!;

        Assert.Equal(EventStatus.Draft, ev.Status);

        var titleResult = EventTitle.Create(exampleTitle);
        Assert.True(titleResult.IsSuccess);

        var updateResult = ev.UpdateTitle(titleResult.Payload!);
        Assert.True(updateResult.IsSuccess);
        Assert.Equal(exampleTitle, ev.Title.Value);
    }

    [Theory]
    [InlineData("Scary Movie Night!")]
    [InlineData("Graduation Gala")]
    [InlineData("VIA Hackathon")]
    public void Id2_S2_TitleUpdate_WhenReady_ShouldAlsoSetStatusToDraft(string exampleTitle)
    {
        var ev = VeaEvent.Create().Payload!;

        // First, we need to make the event Ready - and that is predicated by certain steps. SEE USE CASE ID:8
        // To Ready to Event, it needs to be in draft status, and the following data is set with valid values: title, description, times, visibility, maximum guests
        ev.UpdateDescription(EventDescription.Create("Some desc").Payload!);
        DateTime start = DateTime.Today.AddYears(1).AddHours(8);
        DateTime end = DateTime.Today.AddYears(1).AddHours(9);
        ev.UpdateTimeRange(EventTimeRange.Create(start, end).Payload!);
        ev.UpdateMaxGuests(MaxGuests.Create(10).Payload!);
        ev.UpdateVisibility(EventVisibility.Public);
        ev.Ready();

        Assert.Equal(EventStatus.Ready, ev.Status);

        var titleResult = EventTitle.Create(exampleTitle);
        var updateResult = ev.UpdateTitle(titleResult.Payload!);
        Assert.True(updateResult.IsSuccess);
        Assert.Equal(exampleTitle, ev.Title.Value);
        Assert.Equal(EventStatus.Draft, ev.Status);
    }

    [Theory]
    [InlineData("")]
    public void Id2_F1_TitleEmpty_ShouldFail(string input)
    {
        var result = EventTitle.Create(input);
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventTitleMustBeBetween3And75Characters);
    }

    [Theory]
    [InlineData("XY")]
    [InlineData("a")]
    public void Id2_F2_TitleTooShort_ShouldFail(string input)
    {
        var result = EventTitle.Create(input);
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventTitleMustBeBetween3And75Characters);
    }

    [Fact]
    public void Id2_F3_TitleTooLong_ShouldFail()
    {
        var input = new string('a', 76);
        var result = EventTitle.Create(input);
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventTitleMustBeBetween3And75Characters);
    }

    [Fact]
    public void Id2_F4_TitleIsNull_ShouldFail()
    {
        string? input = null;
        var result = EventTitle.Create(input!); // Force null to test logic
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventTitleMustBeBetween3And75Characters);
    }

    [Fact]
    public void Id2_F5_TitleUpdateOnActiveEvent_ShouldFail()
    {
        // First, we need to make the event Active - and that is predicated by certain steps. SEE USE CASE ID:9
        // To Activate the Event, it needs to be in draft status, and the following data is set with valid values: title, description, times, visibility, maximum guests
        var ev = VeaEvent.Create().Payload!;
        ev.Activate();

        var title = EventTitle.Create("New Title").Payload!;
        var result = ev.UpdateTitle(title);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e == Error.ActiveOrCanceledEventCannotBeModified);
    }

    [Fact]
    public void Id2_F6_TitleUpdateOnCancelledEvent_ShouldFail()
    {
        var ev = VeaEvent.Create().Payload!;
        var cancelResult = ev.Cancel();
        Assert.True(cancelResult.IsSuccess);

        var title = EventTitle.Create("New Title").Payload!;
        var result = ev.UpdateTitle(title);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.ActiveOrCanceledEventCannotBeModified);
    }
}