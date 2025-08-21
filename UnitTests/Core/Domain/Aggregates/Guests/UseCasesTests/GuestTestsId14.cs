using UnitTests.Fakes;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Domain.Aggregates.Guests.UseCasesTests;

public class GuestTestsId14
{
    private static readonly ISystemTime FakeSystemTime = new FakeSystemTime(new DateTime(2023, 8, 10, 12, 0, 0));
    DateTime today = new DateTime(FakeSystemTime.Now().Year, FakeSystemTime.Now().Month, FakeSystemTime.Now().Day, 0, 0, 0);

    [Fact]
    public void Id14_S1_SuccessfullyAcceptInvitation_WhenAllConditionsMet()
    {
        var guest = GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .Build();

        var result = veaEvent.InviteGuest(guest.Id);
        Assert.True(result.IsSuccess);

        result = veaEvent.AcceptInvitation(guest.Id, FakeSystemTime);

        Assert.True(result.IsSuccess);
        Assert.True(veaEvent.HasAcceptedInvitation(guest.Id));
    }

    [Fact]
    public void Id14_F1_Failure_WhenInvitationNotFound()
    {
        var guest = GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .Build();

        var result = veaEvent.AcceptInvitation(guest.Id, FakeSystemTime);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.InvitationNotFound);
    }

    [Fact]
    public void Id14_F2_Failure_WhenTooManyGuests()
    {
        var guest = GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithMaxGuests(1)
            .WithInvitedGuest(guest.Id)
            .WithGuest(GuestId.CreateUnique())
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .Build();

        var result = veaEvent.AcceptInvitation(guest.Id, FakeSystemTime);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.NoMoreRoom);
    }

    [Fact]
    public void Id14_F3_Failure_WhenEventIsCancelled()
    {
        var guest = GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Cancelled)
            .WithInvitedGuest(guest.Id)
            .Build();

        var result = veaEvent.AcceptInvitation(guest.Id, FakeSystemTime);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.CancelledEventsCannotBeJoined);
    }

    [Fact]
    public void Id14_F4_Failure_WhenEventIsReady()
    {
        var guest = GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Ready)
            .WithInvitedGuest(guest.Id)
            .Build();

        var result = veaEvent.AcceptInvitation(guest.Id, FakeSystemTime);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.JoinUnstartedEventImpossible);
    }

    [Fact]
    public void Id14_F5_Failure_WhenEventIsInThePast()
    {
        var guest = GuestFactory.Init().Build();

        var pastStartTime = EventTimeRange.Default(FakeSystemTime).StartTime.AddDays(-2);
        var pastTimeRange = EventTimeRange.Create(pastStartTime, pastStartTime.AddHours(2)).Payload!;

        var veaEvent = EventFactory.Init()
            .WithTimeRange(pastTimeRange)
            .WithStatus(EventStatus.Active)
            .WithInvitedGuest(guest.Id)
            .Build();

        var result = veaEvent.AcceptInvitation(guest.Id, FakeSystemTime);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.TooLate);
    }
}