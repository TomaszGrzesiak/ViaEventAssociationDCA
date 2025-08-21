using UnitTests.Fakes;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Domain.Aggregates.Guests.UseCasesTests;

public class EventTestsId11
{
    private static readonly ISystemTime FakeSystemTime = new FakeSystemTime(new DateTime(2023, 8, 10, 12, 0, 0));

    [Fact]
    public void Id11_S1_SuccessfullyRegisterGuest_ForPublicActiveEvent()
    {
        var guest = GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithVisibility(EventVisibility.Public)
            .WithMaxGuests(10)
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .Build();

        var result = veaEvent.Participate(guest.Id, FakeSystemTime);

        Assert.True(result.IsSuccess);
        Assert.Contains(veaEvent.GuestList, id => id == guest.Id);
    }

    [Theory]
    [InlineData("Draft")]
    [InlineData("Ready")]
    [InlineData("Cancelled")]
    public void Id11_F1_Failure_WhenEventIsNotActive(string statusString)
    {
        var status = EventStatus.FromName(statusString).Payload!;
        var guest = GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithStatus(status)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var result = veaEvent.Participate(guest.Id, FakeSystemTime);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.OnlyActiveEventsCanBeJoined);
    }

    [Fact]
    public void Id11_F2_Failure_WhenEventIsFull()
    {
        var guest = GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithVisibility(EventVisibility.Public)
            .WithMaxGuests(1)
            .WithGuest(GuestId.CreateUnique()) // already 1 guest
            .Build();

        var result = veaEvent.Participate(guest.Id, FakeSystemTime);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.NoMoreRoom);
    }

    [Fact]
    public void Id11_F3_Failure_WhenEventAlreadyStarted()
    {
        var guest = GuestFactory.Init().Build();
        var time = EventTimeRange.Default(FakeSystemTime);
        var pastTime = EventTimeRange.Create(
            time.StartTime.AddDays(-2),
            time.EndTime.AddDays(-2)
        ).Payload!;

        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithVisibility(EventVisibility.Public)
            .WithTimeRange(pastTime)
            .Build();

        var result = veaEvent.Participate(guest.Id, FakeSystemTime);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.TooLate);
    }

    [Fact]
    public void Id11_F4_Failure_WhenEventIsPrivate()
    {
        var guest = GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithVisibility(EventVisibility.Private)
            .Build();

        var result = veaEvent.Participate(guest.Id, FakeSystemTime);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventIsNotPublic);
    }

    [Fact]
    public void Id11_F5_Failure_WhenGuestAlreadyParticipating()
    {
        var guest = GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithGuest(guest.Id)
            .Build();

        var result = veaEvent.Participate(guest.Id, FakeSystemTime);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.GuestAlreadyJoined);
    }
}