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

public class GuestAcceptsInvitationHandlerTests
{
    private static readonly ISystemTime FakeSystemTime = new FakeSystemTime(new DateTime(2023, 8, 10, 12, 0, 0));

    [Fact]
    public async void Id11_S1_GuestIsSuccessfullyAcceptsInvitation_ForPrivateActiveEvent()
    {
        // Arrange
        var eventRepo = new InMemEventRepoStub();
        var guestRepo = new InMemGuestRepoStub();
        var uow = new FakeUoW();
        var handler = new GuestAcceptsInvitationHandler(eventRepo, guestRepo, uow, FakeSystemTime);

        // creating guest and event
        var guestId = GuestId.CreateUnique();
        var guest = await GuestFactory.Init().WithId(guestId.ToString()).Build();

        var eventId = EventId.CreateUnique();
        var ev = EventFactory.Init()
            .WithId(eventId)
            .WithStatus(EventStatus.Active)
            .WithVisibility(EventVisibility.Private)
            .WithMaxGuests(10)
            .WithInvitedGuest(guestId)
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .Build();

        // adding temporary guest and event to repo
        await guestRepo.AddAsync(guest);
        await eventRepo.AddAsync(ev);

        var cmd = GuestAcceptsInvitationCommand.Create(eventId.ToString(), guestId.ToString()).Payload!;

        Assert.Contains(ev.Invitations, i =>
            i.GuestId == guest.Id && Equals(i.Status, InvitationStatus.Pending));
        // Act - business change through handler
        var result = await handler.HandleAsync(cmd);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(ev.Invitations, i =>
            i.GuestId == guest.Id && Equals(i.Status, InvitationStatus.Accepted));
    }

    [Fact]
    public async void Id11_F_GuestFailsToAcceptInvitation_ForPrivateActiveEvent_When_EventNotFound()
    {
        // Arrange
        var eventRepo = new InMemEventRepoStub();
        var guestRepo = new InMemGuestRepoStub();
        var uow = new FakeUoW();
        var handler = new GuestAcceptsInvitationHandler(eventRepo, guestRepo, uow, FakeSystemTime);

        // creating guest and event
        var guestId = GuestId.CreateUnique();
        var guest = await GuestFactory.Init().WithId(guestId.ToString()).Build();

        var eventId = EventId.CreateUnique();
        var ev = EventFactory.Init()
            .WithId(eventId)
            .WithStatus(EventStatus.Active)
            .WithVisibility(EventVisibility.Private)
            .WithMaxGuests(10)
            .WithInvitedGuest(guestId)
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .Build();

        // adding temporary guest and event to repo
        await guestRepo.AddAsync(guest);
        await eventRepo.AddAsync(ev);

        var wrongEventId = EventId.CreateUnique();
        var cmd = GuestAcceptsInvitationCommand.Create(wrongEventId.ToString(), guestId.ToString()).Payload!;

        Assert.Contains(ev.Invitations, i =>
            i.GuestId == guest.Id && Equals(i.Status, InvitationStatus.Pending));
        // Act - business change through handler
        var result = await handler.HandleAsync(cmd);

        Assert.True(result.IsFailure);
        Assert.Contains(Error.EventNotFound, result.Errors);
    }

    [Fact]
    public async void Id11_F_GuestFailsToAcceptInvitation_ForPrivateActiveEvent_When_GuestNotFound()
    {
        // Arrange
        var eventRepo = new InMemEventRepoStub();
        var guestRepo = new InMemGuestRepoStub();
        var uow = new FakeUoW();
        var handler = new GuestAcceptsInvitationHandler(eventRepo, guestRepo, uow, FakeSystemTime);

        // creating guest and event
        var guestId = GuestId.CreateUnique();
        var guest = await GuestFactory.Init().WithId(guestId.ToString()).Build();

        var eventId = EventId.CreateUnique();
        var ev = EventFactory.Init()
            .WithId(eventId)
            .WithStatus(EventStatus.Active)
            .WithVisibility(EventVisibility.Private)
            .WithMaxGuests(10)
            .WithInvitedGuest(guestId)
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .Build();

        // adding temporary guest and event to repo
        await guestRepo.AddAsync(guest);
        await eventRepo.AddAsync(ev);

        var wrongGuestId = GuestId.CreateUnique();
        var cmd = GuestAcceptsInvitationCommand.Create(eventId.ToString(), wrongGuestId.ToString()).Payload!;

        Assert.Contains(ev.Invitations, i =>
            i.GuestId == guest.Id && Equals(i.Status, InvitationStatus.Pending));
        // Act - business change through handler
        var result = await handler.HandleAsync(cmd);

        Assert.True(result.IsFailure);
        Assert.Contains(Error.GuestNotFound, result.Errors);
    }

    [Fact]
    public async void Id11_F_GuestFailsToAcceptInvitation_ForPrivateActiveEvent_When_WhenInvitationNotFound()
    {
        // Arrange
        var eventRepo = new InMemEventRepoStub();
        var guestRepo = new InMemGuestRepoStub();
        var uow = new FakeUoW();
        var handler = new GuestAcceptsInvitationHandler(eventRepo, guestRepo, uow, FakeSystemTime);

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

        var cmd = GuestAcceptsInvitationCommand.Create(eventId.ToString(), guestId.ToString()).Payload!;

        // Act - business change through handler
        var result = await handler.HandleAsync(cmd);

        Assert.True(result.IsFailure);
        Assert.Contains(Error.InvitationNotFound, result.Errors);
    }
}