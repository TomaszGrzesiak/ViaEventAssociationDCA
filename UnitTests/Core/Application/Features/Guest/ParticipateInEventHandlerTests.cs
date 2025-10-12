using UnitTests.Fakes;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Guest;
using ViaEventAssociation.Core.Application.Features.Guest;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Application.Features.Guest;

public class ParticipateInEventHandlerTests
{
    private static readonly ISystemTime FakeSystemTime = new FakeSystemTime(new DateTime(2023, 8, 10, 12, 0, 0));

    [Fact]
    public async void Id11_S1_GuestSuccessfullyParticipates_ForPublicActiveEvent()
    {
        // Arrange
        var eventRepo = new InMemEventRepoStub();
        var guestRepo = new InMemGuestRepoStub();
        var uow = new FakeUoW();
        var handler = new ParticipateInEventHandler(eventRepo, guestRepo, uow, FakeSystemTime);

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
            .Build();

        // adding guest and event to repo
        await guestRepo.AddAsync(guest);
        await eventRepo.AddAsync(ev);

        var cmd = ParticipateInEventCommand.Create(eventId.ToString(), guestId.ToString()).Payload!;

        // Act - business change through handler
        var result = await handler.HandleAsync(cmd);

        // Assert - let's see if the handler added the arranged guest to the arranged event
        Assert.True(result.IsSuccess);
        Assert.Contains(ev.GuestList, id => id == guest.Id);
    }

    [Fact]
    public async void Id11_F_GuestParticipates_ForPublicActiveEvent_Fails_When_Wrong_Event_Id()
    {
        // Arrange
        var eventRepo = new InMemEventRepoStub();
        var guestRepo = new InMemGuestRepoStub();
        var uow = new FakeUoW();
        var handler = new ParticipateInEventHandler(eventRepo, guestRepo, uow, FakeSystemTime);

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
            .Build();

        // adding guest and event to repo
        await guestRepo.AddAsync(guest);
        await eventRepo.AddAsync(ev);

        var wrongEventId = EventId.CreateUnique();
        var cmd = ParticipateInEventCommand.Create(wrongEventId.ToString(), guestId.ToString()).Payload!;

        // Act - business change through handler
        var result = await handler.HandleAsync(cmd);

        Assert.True(result.IsFailure);
        Assert.Contains(Error.EventNotFound, result.Errors);
    }

    [Fact]
    public async void Id11_F_GuestParticipates_ForPublicActiveEvent_Fails_When_Wrong_Guest_Id()
    {
        // Arrange
        var eventRepo = new InMemEventRepoStub();
        var guestRepo = new InMemGuestRepoStub();
        var uow = new FakeUoW();
        var handler = new ParticipateInEventHandler(eventRepo, guestRepo, uow, FakeSystemTime);

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
            .Build();

        // adding guest and event to repo
        await guestRepo.AddAsync(guest);
        await eventRepo.AddAsync(ev);

        var wrongGuestId = GuestId.CreateUnique();
        var cmd = ParticipateInEventCommand.Create(eventId.ToString(), wrongGuestId.ToString()).Payload!;

        // Act - business change through handler
        var result = await handler.HandleAsync(cmd);

        Assert.True(result.IsFailure);
        Assert.Contains(Error.GuestNotFound, result.Errors);
    }

    [Fact]
    public async void Id11_F3_GuestParticipates_ForPublicActiveEvent_Fails_When_EventHasAlreadyStarted()
    {
        // Arrange
        var eventRepo = new InMemEventRepoStub();
        var guestRepo = new InMemGuestRepoStub();
        var uow = new FakeUoW();
        var handler = new ParticipateInEventHandler(eventRepo, guestRepo, uow, FakeSystemTime);

        // creating guest and event
        var guestId = GuestId.CreateUnique();
        var guest = await GuestFactory.Init().WithId(guestId.ToString()).Build();

        // ensuring the event has starting time in the past
        var time = EventTimeRange.Default(FakeSystemTime);
        var pastTime = EventTimeRange.Create(
            time.StartTime.AddDays(-2),
            time.EndTime.AddDays(-2)
        ).Payload!;

        var eventId = EventId.CreateUnique();
        var ev = EventFactory.Init()
            .WithId(eventId)
            .WithStatus(EventStatus.Active)
            .WithVisibility(EventVisibility.Public)
            .WithMaxGuests(10)
            .WithTimeRange(pastTime)
            .Build();

        // adding guest and event to repo
        await guestRepo.AddAsync(guest);
        await eventRepo.AddAsync(ev);

        var cmd = ParticipateInEventCommand.Create(eventId.ToString(), guestId.ToString()).Payload!;

        // Act - business change through handler
        var result = await handler.HandleAsync(cmd);

        Assert.True(result.IsFailure);
        Assert.Contains(Error.TooLate, result.Errors);
    }
}