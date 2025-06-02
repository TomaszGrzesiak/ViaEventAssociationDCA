using UnitTests.Helpers;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Domain.Aggregates.Events.UseCasesTests;

public class EventTestsId8
{
    [Fact]
    public void Id8_S1_SuccessfullyReadiesEvent_WhenAllFieldsValid()
    {
        var timeRange = EventTimeRange.Default();
        var veaEvent = EventFactory.Init()
            .WithTitle("Summer Party")
            .WithDescription("Join us for a summer celebration!")
            .WithTimeRange(timeRange)
            .WithMaxGuests(25)
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Draft)
            .Build();

        var result = veaEvent.Ready();

        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Ready, veaEvent.Status);
    }

    // [Theory]
    // [InlineData("DefaultTitle", "Valid description", true, 25, "Title must be changed")]
    // [InlineData("Valid Title", "DefaultDescription", true, 25, "Description must be changed")]
    // [InlineData("Valid Title", "Valid Description", false, 25, "Time must be set")]
    // [InlineData("Valid Title", "Valid Description", true, 3, "Guest count too low")]
    // [InlineData("Valid Title", "Valid Description", true, 51, "Guest count too high")]
    // public void Id8_F1_Failure_WhenFieldsAreInvalid(string titleText, string descText, bool setTime, int guests, string reason)
    // {
    //     var timeRange = setTime ? EventTimeRange.Default() : null;
    //
    //     var veaEvent = EventFactory.Init()
    //         .WithTitle(titleText)
    //         .WithDescription(descText)
    //         .WithTimeRange(timeRange)
    //         .WithMaxGuests(guests)
    //         .WithVisibility(EventVisibility.Public)
    //         .WithStatus(EventStatus.Draft)
    //         .Build();
    //
    //     var result = veaEvent.Ready();
    //
    //     Assert.False(result.IsSuccess);
    // }

    [Fact]
    public void Id8_F1_Failure_WhenFieldsAreInvalid()
    {
        var validDraftEvent = EventFactory.Init()
            .WithStatus(EventStatus.Draft)
            .WithValidTitle()
            .WithValidDescription()
            .WithTimeRange(EventTimeRange.ValidNonDefault())
            .WithMaxGuests(25)
            .Build();

        // messing title
        var messUpdate = validDraftEvent.UpdateTitle(EventTitle.Default());
        Assert.True(messUpdate.IsSuccess);
        var result = validDraftEvent.Ready();
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e == Error.EventTitleCannotBeDefault);

        // messing description
        messUpdate = validDraftEvent.UpdateDescription(EventDescription.Default());
        Assert.True(messUpdate.IsSuccess);
        result = validDraftEvent.Ready();
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e == Error.EventDescriptionCannotBeDefault);

        // messing times
        messUpdate = validDraftEvent.UpdateTimeRange(EventTimeRange.Default());
        Assert.True(messUpdate.IsSuccess);
        result = validDraftEvent.Ready();
        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e == Error.EventTimeRangeCannotBeDefault);

        // messing visibility
        // messUpdate = validDraftEvent.UpdateVisibility(null);
        // Assert.True(messUpdate.IsSuccess);
        // result = validDraftEvent.Ready();
        // Assert.False(result.IsSuccess);
        // Assert.Contains(result.Errors, e => e == Error.EventTimeRangeCannotBeDefault);
    }


    [Fact]
    public void Id8_F2_Failure_WhenEventIsCancelled()
    {
        var veaEvent = EventFactory.Init()
            .WithTitle("Event")
            .WithDescription("Description")
            .WithTimeRange(EventTimeRange.Default())
            .WithMaxGuests(25)
            .WithVisibility(EventVisibility.Private)
            .WithStatus(EventStatus.Cancelled)
            .Build();

        var result = veaEvent.Ready();

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventAlreadyCancelled);
    }

    // [Fact]
    // public void Id8_F3_Failure_WhenStartTimeIsInThePast()
    // {
    //     var defaultRange = EventTimeRange.Default();
    //     var pastRange = EventTimeRange.Create(
    //         defaultRange.StartTime.AddDays(-2),
    //         defaultRange.EndTime.AddDays(-2)).Payload!;
    //
    //     var veaEvent = EventFactory.Init()
    //         .WithTitle("Title")
    //         .WithDescription("Description")
    //         .WithTimeRange(pastRange)
    //         .WithMaxGuests(25)
    //         .WithVisibility(EventVisibility.Private)
    //         .WithStatus(EventStatus.Draft)
    //         .Build();
    //
    //     var result = veaEvent.Ready();
    //
    //     Assert.True(result.IsFailure);
    //     Assert.Contains(result.Errors, e => e == Error.EventTimeCannotStartInPast);
    // }

    [Fact]
    public void Id8_F4_Failure_WhenTitleIsDefault()
    {
        var veaEvent = EventFactory.Init()
            .WithTitle(EventTitle.Default().ToString())
            .WithDescription("Description")
            .WithTimeRange(EventTimeRange.Default())
            .WithMaxGuests(25)
            .WithVisibility(EventVisibility.Private)
            .WithStatus(EventStatus.Draft)
            .Build();

        var result = veaEvent.Ready();

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventTitleCannotBeDefault);
    }
}