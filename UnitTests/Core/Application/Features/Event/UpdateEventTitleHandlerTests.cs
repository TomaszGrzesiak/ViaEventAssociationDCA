using UnitTests.Fakes;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Application.Features.Event;

public class UpdateEventTitleHandlerTests
{
    [Fact]
    public async Task HandleAsync_Updates_Title_When_Event_Exists()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var handler = new UpdateEventTitleHandler(repo, uow);

        string newTitle = "Scary Movie Night!";

        var id = EventId.FromGuid(Guid.NewGuid());
        var ev = EventFactory.Init()
            .WithId(id).Build();

        await repo.AddAsync(ev);

        var cmd = UpdateEventTitleCommand.Create(id.ToString(), newTitle).Payload!;
        var result = await handler.HandleAsync(cmd);

        Assert.True(result.IsSuccess);

        var reloaded = await repo.GetAsync(id);
        Assert.NotNull(reloaded);
        Assert.Equal(newTitle, reloaded!.Title?.ToString());
    }

    [Fact]
    public async Task HandleAsync_Fails_When_Event_Not_Found()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var handler = new UpdateEventTitleHandler(repo, uow);

        var missingId = EventId.FromGuid(Guid.NewGuid());
        var cmd = UpdateEventTitleCommand.Create(missingId.ToString(), "New Title").Payload!;

        var result = await handler.HandleAsync(cmd);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventNotFound);
    }

    [Fact]
    public async Task HandleAsync_Succeeds_WhenDraftOrReady()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var handler = new UpdateEventTitleHandler(repo, uow);

        var id = EventId.FromGuid(Guid.NewGuid());
        var ev = EventFactory.Init()
            .WithId(id)
            .WithStatus(EventStatus.Ready).Build();

        await repo.AddAsync(ev);

        var cmd = UpdateEventTitleCommand.Create(id.ToString(), "Graduation Gala").Payload!;
        var result = await handler.HandleAsync(cmd);

        Assert.True(result.IsSuccess);

        var reloaded = await repo.GetAsync(id);
        Assert.Equal("Graduation Gala", reloaded!.Title!.Value);
        Assert.Equal(EventStatus.Draft, reloaded.Status); // Ready -> Draft after update
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("XY")]
    public async Task HandleAsync_Fails_WhenTitleInvalid(string badTitle)
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var handler = new UpdateEventTitleHandler(repo, uow);

        var id = EventId.FromGuid(Guid.NewGuid());
        var ev = EventFactory.Init()
            .WithId(id)
            .WithStatus(EventStatus.Ready).Build();
        await repo.AddAsync(ev);

        var result = UpdateEventTitleCommand.Create(id.ToString(), badTitle);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventTitleMustBeBetween3And75Characters);
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenEventActive()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var handler = new UpdateEventTitleHandler(repo, uow);

        var ev = EventFactory.Init().WithStatus(EventStatus.Active).Build();
        await repo.AddAsync(ev);

        var cmd = UpdateEventTitleCommand.Create(ev.Id.ToString(), "New Title").Payload!;
        var result = await handler.HandleAsync(cmd);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.ActiveOrCanceledEventCannotBeModified);
    }

    [Fact]
    public async Task HandleAsync_Fails_WhenEventCancelled()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var handler = new UpdateEventTitleHandler(repo, uow);
        var eventId = EventId.FromGuid(Guid.NewGuid());

        var ev = VeaEvent.Create(eventId).Payload!;
        ev.Cancel();
        await repo.AddAsync(ev);

        var cmd = UpdateEventTitleCommand.Create(ev.Id.ToString(), "New Title").Payload!;
        var result = await handler.HandleAsync(cmd);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.ActiveOrCanceledEventCannotBeModified);
    }
}