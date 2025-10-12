using UnitTests.Helpers;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Domain.Aggregates.Events.UseCasesTests;

public class EventTestsId7
{
    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(25)]
    [InlineData(50)]
    public void Id7_S1_S2_SuccessfullyUpdateMaxGuests_WhenStatusIsDraftOrReady(int newLimit)
    {
        foreach (var status in new[] { EventStatus.Draft, EventStatus.Ready })
        {
            var veaEvent = EventFactory.Init()
                .WithMaxGuests(5)
                .WithStatus(status)
                .Build();

            var result = MaxGuests.Create(newLimit);
            var newMax = result.Payload;

            var result2 = veaEvent.UpdateMaxGuests(newMax!);

            Assert.True(result2.IsSuccess);
            Assert.Equal(newLimit, veaEvent.MaxGuestsNo.Value);
        }
    }

    [Fact]
    public void Id7_S3_SuccessfullyIncreaseMaxGuests_WhenActive()
    {
        var maxGuest = 10;
        var veaEvent = EventFactory.Init()
            .WithMaxGuests(maxGuest)
            .WithStatus(EventStatus.Active)
            .Build();


        var result = veaEvent.UpdateMaxGuests(MaxGuests.Create(maxGuest + 1).Payload!);

        Assert.True(result.IsSuccess);
        Assert.True(veaEvent.MaxGuestsNo.Value == maxGuest + 1);
    }

    [Fact]
    public void Id7_F1_CannotReduceGuests_WhenActive()
    {
        var veaEvent = EventFactory.Init()
            .WithMaxGuests(25)
            .WithStatus(EventStatus.Active)
            .Build();

        var result = veaEvent.UpdateMaxGuests(MaxGuests.Create(10).Payload!);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.DecreaseMaxGuestsImpossible);
        Assert.Contains(result.Errors, e => e == Error.EventAlreadyActive);
    }

    [Fact]
    public void Id7_F2_CannotUpdateGuests_WhenCancelled()
    {
        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Cancelled)
            .Build();

        var result = veaEvent.UpdateMaxGuests(MaxGuests.Create(25).Payload!);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.UpdateMaxGuestsImpossible);
        Assert.Contains(result.Errors, e => e == Error.EventAlreadyCancelled);
    }


    [Fact]
    public void Id7_F3_Failure_WhenExceedingLocationCapacity()
    {
        int locationMax = 40;
        var veaEvent = EventFactory.Init()
            .WithLocationMaxCapacity(locationMax)
            .Build();
        var result = veaEvent.UpdateMaxGuests(MaxGuests.Create(locationMax + 1).Payload!);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.UpdateMaxGuestsImpossible);
        Assert.Contains(result.Errors, e => e == Error.MaxGuestAboveLocationCapacity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    public void Id7_F4_Failure_WhenBelowMinimumLimit(int value)
    {
        var result = MaxGuests.Create(value);
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.GuestsMaxNumberTooSmall);
    }

    [Fact]
    public void Id7_F5_Failure_WhenAboveMaximumLimit()
    {
        var value = 51;
        var result = MaxGuests.Create(value);
        Assert.True(result.IsFailure);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.GuestsMaxNumberTooGreat);
    }
}