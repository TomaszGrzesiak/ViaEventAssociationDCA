using UnitTests.Fakes;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Guest;
using ViaEventAssociation.Core.Application.Features.Guest;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Events.Entities;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Application.Features.Guest;

public class InviteGuestHandlerTests
{
    private static readonly ISystemTime FakeSystemTime = new FakeSystemTime(new DateTime(2023, 8, 10, 12, 0, 0));

    [Fact]
    public async void Id11_S1_GuestIsSuccessfullyInvited_ForPrivateActiveEvent()
    {
        // Arrange
        var eventRepo = new InMemEventRepoStub();
        var guestRepo = new InMemGuestRepoStub();
        var uow = new FakeUoW();
        var handler = new InviteGuestHandler(eventRepo, guestRepo, uow);

        // creating guest and event
        var guestId = GuestId.CreateUnique();
        var guest = await GuestFactory.Init().WithId(guestId.ToString()).Build();

        var eventId = EventId.CreateUnique();
        var ev = EventFactory.Init()
            .WithId(eventId)
            .WithStatus(EventStatus.Active)
            .WithVisibility(EventVisibility.Private)
            .WithMaxGuests(10)
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .Build();

        // adding temporary guest and event to repo
        await guestRepo.AddAsync(guest);
        await eventRepo.AddAsync(ev);

        var cmd = InviteGuestCommand.Create(eventId.ToString(), guestId.ToString()).Payload!;

        // Act - business change through handler
        var result = await handler.HandleAsync(cmd);

        // Assert
        Assert.True(result.IsSuccess);
        // Ensure an Invitation was added for this guest and is Pending
        Assert.Contains(ev.Invitations, i =>
            i.GuestId == guest.Id && Equals(i.Status, InvitationStatus.Pending));

        // (Optional) ensure not duplicated
        Assert.Equal(1, ev.Invitations.Count(i => i.GuestId == guest.Id));
    }

    [Fact]
    public async void Id11_F_Guest_Fails_To_Be_Invited_ForPrivateEvent_When_EventNotFound()
    {
        // Arrange
        var eventRepo = new InMemEventRepoStub();
        var guestRepo = new InMemGuestRepoStub();
        var uow = new FakeUoW();
        var handler = new InviteGuestHandler(eventRepo, guestRepo, uow);

        // creating guest and event
        var guestId = GuestId.CreateUnique();
        var guest = await GuestFactory.Init().WithId(guestId.ToString()).Build();

        var eventId = EventId.CreateUnique();
        var ev = EventFactory.Init()
            .WithId(eventId)
            .WithStatus(EventStatus.Active)
            .WithVisibility(EventVisibility.Private)
            .WithMaxGuests(10)
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .Build();

        // adding temporary guest and event to repo
        await guestRepo.AddAsync(guest);
        await eventRepo.AddAsync(ev);

        var wrongEventId = EventId.CreateUnique();
        var cmd = InviteGuestCommand.Create(wrongEventId.ToString(), guestId.ToString()).Payload!;

        // Act - business change through handler
        var result = await handler.HandleAsync(cmd);

        // Assert
        Assert.True(result.IsFailure);
        // Ensure an Invitation was added for this guest and is Pending
        Assert.Contains(Error.EventNotFound, result.Errors);
    }


    [Fact]
    public async void Id11_F_Guest_Fails_To_Be_Invited_ForPrivateEvent_When_Wrong_Guest_Id()
    {
        // Arrange
        var eventRepo = new InMemEventRepoStub();
        var guestRepo = new InMemGuestRepoStub();
        var uow = new FakeUoW();
        var handler = new InviteGuestHandler(eventRepo, guestRepo, uow);

        // creating guest and event
        var guestId = GuestId.CreateUnique();
        var guest = await GuestFactory.Init().WithId(guestId.ToString()).Build();

        var eventId = EventId.CreateUnique();
        var ev = EventFactory.Init()
            .WithId(eventId)
            .WithStatus(EventStatus.Active)
            .WithVisibility(EventVisibility.Private)
            .WithMaxGuests(10)
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .Build();

        // adding temporary guest and event to repo
        await guestRepo.AddAsync(guest);
        await eventRepo.AddAsync(ev);

        var wrongGuestId = EventId.CreateUnique();
        var cmd = InviteGuestCommand.Create(eventId.ToString(), wrongGuestId.ToString()).Payload!;

        // Act - business change through handler
        var result = await handler.HandleAsync(cmd);

        // Assert
        Assert.True(result.IsFailure);
        // Ensure an Invitation was added for this guest and is Pending
        Assert.Contains(Error.GuestNotFound, result.Errors);
    }

    [Fact]
    public async void Id11_F4_Guest_Fails_To_Be_Invited_ForPrivateEvent_When_GuestIsAlreadyParticipating()
    {
        // Arrange
        var eventRepo = new InMemEventRepoStub();
        var guestRepo = new InMemGuestRepoStub();
        var uow = new FakeUoW();
        var handler = new InviteGuestHandler(eventRepo, guestRepo, uow);

        // creating guest and event
        var guestId = GuestId.CreateUnique();
        var guest = await GuestFactory.Init().WithId(guestId.ToString()).Build();

        var eventId = EventId.CreateUnique();
        var ev = EventFactory.Init()
            .WithId(eventId)
            .WithStatus(EventStatus.Active)
            .WithVisibility(EventVisibility.Private)
            .WithMaxGuests(10)
            .WithGuest(guestId)
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .Build();

        // adding temporary guest and event to repo
        await guestRepo.AddAsync(guest);
        await eventRepo.AddAsync(ev);

        var cmd = InviteGuestCommand.Create(eventId.ToString(), guestId.ToString()).Payload!;

        // Act - business change through handler
        var result = await handler.HandleAsync(cmd);

        // Assert
        Assert.True(result.IsFailure);
        // Ensure an Invitation was added for this guest and is Pending
        Assert.Contains(Error.GuestAlreadyJoined, result.Errors);
    }
}