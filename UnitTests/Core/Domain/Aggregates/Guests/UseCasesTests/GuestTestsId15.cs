using UnitTests.Fakes;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Events.Entities;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Domain.Aggregates.Guests.UseCasesTests;

public class GuestTestsId15
{
    private static readonly ISystemTime FakeSystemTime = new FakeSystemTime(new DateTime(2023, 8, 10, 12, 0, 0));

    [Fact]
    public void Id15_S1_Success_WhenInvitationIsPending()
    {
        var guest = GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithInvitedGuest(guest.Id)
            .Build();

        var result = veaEvent.DeclineInvitation(guest.Id);

        Assert.True(result.IsSuccess);
        var invitation = veaEvent.Invitations.FirstOrDefault(i => i.GuestId == guest.Id);
        Assert.NotNull(invitation);
        Assert.Equal(InvitationStatus.Rejected, invitation!.Status);
    }

    [Fact]
    public void Id15_S2_Success_WhenInvitationIsAlreadyAccepted()
    {
        var guest = GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .WithInvitedGuest(guest.Id)
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .Build();

        var result = veaEvent.AcceptInvitation(guest.Id, FakeSystemTime);
        Assert.True(result.IsSuccess);

        result = veaEvent.DeclineInvitation(guest.Id);
        Assert.True(result.IsSuccess);

        var invitation = veaEvent.Invitations.FirstOrDefault(i => i.GuestId == guest.Id);
        Assert.NotNull(invitation);
        Assert.Equal(InvitationStatus.Rejected, invitation!.Status);
    }

    [Fact]
    public void Id15_F1_Failure_WhenInvitationNotFound()
    {
        var guest = GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .Build();

        var result = veaEvent.DeclineInvitation(guest.Id);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e == Error.InvitationNotFound);
    }

    [Fact]
    public void Id15_F2_Failure_WhenEventIsCancelled()
    {
        var guest = GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithInvitedGuest(guest.Id)
            .WithStatus(EventStatus.Cancelled)
            .Build();

        var result = veaEvent.DeclineInvitation(guest.Id);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e == Error.DeclineImpossibleOnCancelledEvent);
    }
}