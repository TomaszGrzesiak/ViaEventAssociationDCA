using UnitTests.Fakes;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Application.Features.Event;

public class ReadyEventHandlerTests
{
    private readonly ISystemTime _fakeSystemTime = new FakeSystemTime(new DateTime(2025, 9, 19, 12, 0, 0));

    [Fact]
    public async void HandleAsync_Ready_Event_Succeeds_When_All_Fields_Are_Valid()
    {
        // Arrange
        var eventRepo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var handler = new ReadyEventHandler(eventRepo, uow, _fakeSystemTime);

        var id = EventId.CreateUnique();
        var ev = EventFactory
            .Init().WithId(id)
            .WithTitle("Valid Title")
            .WithDescription("Valid Description")
            .WithTimeRange(EventTimeRange.Default(_fakeSystemTime))
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Draft)
            .Build();
        await eventRepo.AddAsync(ev);
        var cmd = ReadyEventCommand.Create(id.ToString()).Payload!;

        // Act
        var result = await handler.HandleAsync(cmd);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async void HandleAsync_Ready_Event_Fails_When_EventId_Is_InValid()
    {
        // Arrange
        var eventRepo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var handler = new ReadyEventHandler(eventRepo, uow, _fakeSystemTime);

        var id = EventId.CreateUnique();
        var ev = EventFactory
            .Init().WithId(id)
            .WithTitle("Valid Title")
            .WithDescription("Valid Description")
            .WithTimeRange(EventTimeRange.Default(_fakeSystemTime))
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Draft)
            .Build();
        await eventRepo.AddAsync(ev);

        var newId = EventId.CreateUnique();
        var cmd = ReadyEventCommand.Create(newId.ToString()).Payload!;

        // Act
        var result = await handler.HandleAsync(cmd);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventNotFound);
    }


    // test if the handler grabs domain errors
    [Fact]
    public async void HandleAsync_Ready_Event_Fails_When_TitleIsInvalid()
    {
        // Arrange
        var eventRepo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var handler = new ReadyEventHandler(eventRepo, uow, _fakeSystemTime);

        var id = EventId.CreateUnique();
        var ev = EventFactory
            .Init().WithId(id)
            .WithDescription("Valid Description")
            .WithTimeRange(EventTimeRange.Default(_fakeSystemTime))
            .WithVisibility(EventVisibility.Public)
            .WithStatus(EventStatus.Draft)
            .Build();
        await eventRepo.AddAsync(ev);
        var cmd = ReadyEventCommand.Create(id.ToString()).Payload!;

        // Act
        var result = await handler.HandleAsync(cmd);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventTitleCannotBeDefaultOrEmpty);
    }
}