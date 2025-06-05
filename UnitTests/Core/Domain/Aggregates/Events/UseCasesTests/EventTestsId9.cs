using UnitTests.Helpers;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Domain.Aggregates.Events.UseCasesTests;

public class EventTestsId9
{
    [Fact]
    public void Id9_S1_ActivateEventFromDraft_MakesReadyThenActive()
    {
        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Draft)
            .WithValidTitle()
            .WithValidDescription()
            .WithTimeRange(EventTimeRange.ValidNonDefault())
            .WithVisibility(EventVisibility.Public)
            .WithMaxGuests(10)
            .Build();

        var result = veaEvent.Activate();

        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Active, veaEvent.Status);
    }

    [Fact]
    public void Id9_S2_ActivateEventFromReady_MakesItActive()
    {
        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Ready)
            .Build();

        var result = veaEvent.Activate();

        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Active, veaEvent.Status);
    }

    [Fact]
    public void Id9_S3_ActivateEventFromActive_ChangesNothing()
    {
        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .Build();

        var result = veaEvent.Activate();

        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Active, veaEvent.Status);
    }

    [Fact]
    public void Id9_F1_ActivateFails_WhenFieldsAreInvalid()
    {
        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Draft)
            .WithTitle(null)
            .WithDescription(null)
            .WithTimeRange(null)
            .WithMaxGuests(1)
            .Build();

        var result = veaEvent.Activate();

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.ActivateFailure);
        Assert.Contains(result.Errors, e => e == Error.EventTitleCannotBeDefaultOrEmpty);
        Assert.Contains(result.Errors, e => e == Error.EventDescriptionCannotBeDefault);
        Assert.Contains(result.Errors, e => e == Error.EventTimeRangeMissing);
        Assert.Contains(result.Errors, e => e == Error.GuestsMaxNumberTooSmall);
    }

    [Fact]
    public void Id9_F2_ActivateFails_WhenCancelled()
    {
        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Cancelled)
            .Build();

        var result = veaEvent.Activate();

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventAlreadyCancelled);
    }
}