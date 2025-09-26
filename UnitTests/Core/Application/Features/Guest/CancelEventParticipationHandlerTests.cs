using UnitTests.Fakes;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Guest;
using ViaEventAssociation.Core.Application.Features.Guest;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Application.Features.Guest;

public class CancelEventParticipationHandlerTests
{
    private static readonly ISystemTime FakeSystemTime = new FakeSystemTime(new DateTime(2023, 8, 10, 12, 0, 0));

    [Fact]
    public async void Id12_S1_RemovesParticipation_WhenGuestIsParticipating()
    {
        // Arrange
        var eventRepo = new InMemEventRepoStub();
        var guestRepo = new InMemGuestRepoStub();
        var uow = new FakeUoW();
        var handler = new CancelEventParticipationHandler(eventRepo, guestRepo, uow, FakeSystemTime);

        // creating guest and event
        var guestId = GuestId.CreateUnique();
        var guest = await GuestFactory.Init().WithId(guestId.ToString()).Build();

        var eventId = EventId.CreateUnique();
        var ev = EventFactory.Init()
            .WithId(eventId)
            .WithStatus(EventStatus.Active)
            .WithVisibility(EventVisibility.Public)
            .WithMaxGuests(10)
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .WithGuest(guestId)
            .Build();

        // adding guest and event to repo
        await guestRepo.AddAsync(guest);
        await eventRepo.AddAsync(ev);
        Assert.Contains(ev.GuestList, id => id == guest.Id);

        var cmd = CancelEventParticipationCommand.Create(eventId.ToString(), guestId.ToString()).Payload!;

        // Act - business change through handler
        var result = await handler.HandleAsync(cmd);

        // Assert 
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(ev.GuestList, id => id == guest.Id);
    }

    [Fact]
    public async void Id12_F_RemovesParticipation_Fails_WhenGuestIsNotExisting()
    {
        // Arrange
        var eventRepo = new InMemEventRepoStub();
        var guestRepo = new InMemGuestRepoStub();
        var uow = new FakeUoW();
        var handler = new CancelEventParticipationHandler(eventRepo, guestRepo, uow, FakeSystemTime);

        var eventId = EventId.CreateUnique();
        var ev = EventFactory.Init()
            .WithId(eventId)
            .WithStatus(EventStatus.Active)
            .WithVisibility(EventVisibility.Public)
            .WithMaxGuests(10)
            .WithTimeRange(EventTimeRange.Default(FakeSystemTime))
            .Build();

        // adding guest and event to repo
        await eventRepo.AddAsync(ev);

        var guestId = GuestId.CreateUnique();
        var cmd = CancelEventParticipationCommand.Create(eventId.ToString(), guestId.ToString()).Payload!;

        // Act - business change through handler
        var result = await handler.HandleAsync(cmd);

        // Assert 
        Assert.True(result.IsFailure);
        Assert.Contains(Error.GuestNotFound, result.Errors);
    }

    [Fact]
    public async void Id12_F_RemovesParticipation_Fails_WhenEventIsNotExisting()
    {
        // Arrange
        var eventRepo = new InMemEventRepoStub();
        var guestRepo = new InMemGuestRepoStub();
        var uow = new FakeUoW();
        var handler = new CancelEventParticipationHandler(eventRepo, guestRepo, uow, FakeSystemTime);

        // creating guest and event
        var guestId = GuestId.CreateUnique();
        var guest = await GuestFactory.Init().WithId(guestId.ToString()).Build();

        var eventId = EventId.CreateUnique();

        // adding guest and event to repo
        await guestRepo.AddAsync(guest);

        var cmd = CancelEventParticipationCommand.Create(eventId.ToString(), guestId.ToString()).Payload!;

        // Act - business change through handler
        var result = await handler.HandleAsync(cmd);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(Error.EventNotFound, result.Errors);
    }
}