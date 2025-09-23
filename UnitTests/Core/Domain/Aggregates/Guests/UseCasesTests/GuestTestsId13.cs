using UnitTests.Helpers;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Domain.Aggregates.Guests.UseCasesTests;

public class EventTestsId13
{
    [Theory]
    [InlineData("Ready")]
    [InlineData("Active")]
    public async void Id13_S1_SuccessfullyInviteGuest_WhenEventReadyOrActive(string statusString)
    {
        var status = EventStatus.FromName(statusString).Payload!;
        var guest = await GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithStatus(status)
            .Build();

        var result = veaEvent.InviteGuest(guest.Id);

        Assert.True(result.IsSuccess);
        Assert.Contains(veaEvent.Invitations, i => i.GuestId == guest.Id);
    }

    [Theory]
    [InlineData("Draft")]
    [InlineData("Cancelled")]
    public async void Id13_F1_Failure_WhenEventIsDraftOrCancelled(string statusString)
    {
        var status = EventStatus.FromName(statusString).Payload!;
        var guest = await GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithStatus(status)
            .Build();

        var result = veaEvent.InviteGuest(guest.Id);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.CanOnlyInviteToReadyOrActiveEvent);
    }

    [Fact]
    public async void Id13_F2_Failure_WhenEventIsFull()
    {
        var guest = await GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithMaxGuests(5)
            .WithStatus(EventStatus.Active)
            .WithGhostGuests(5)
            .Build();

        var result = veaEvent.InviteGuest(guest.Id);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.NoMoreRoom);
    }

    [Fact]
    public async void Id13_F3_Failure_WhenGuestAlreadyInvited()
    {
        var guest = await GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithStatus(EventStatus.Active)
            .Build();

        // invite once
        var result = veaEvent.InviteGuest(guest.Id);
        Assert.True(result.IsSuccess);

        // invite again
        result = veaEvent.InviteGuest(guest.Id);
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.GuestAlreadyInvited);
    }

    [Fact]
    public async void Id13_F4_Failure_WhenGuestAlreadyParticipating()
    {
        var guest = await GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithGuest(guest.Id)
            .WithStatus(EventStatus.Active)
            .Build();

        var result = veaEvent.InviteGuest(guest.Id);
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.GuestAlreadyJoined);
    }
}