using UnitTests.Fakes;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Application.Features.Event;

public class UpdateEventDescriptionHandlerTests
{
    [Fact]
    public async Task HandleAsync_Updates_Description()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var handler = new UpdateEventDescriptionHandler(repo, uow);

        var id = EventId.FromGuid(Guid.NewGuid());
        var ev = EventFactory.Init().WithId(id).Build();

        await repo.AddAsync(ev);

        string desc = "Snacks provided. Bring your own alcohol.";
        var cmdResult = UpdateEventDescriptionCommand.Create(id.ToString(), desc);
        Assert.True(cmdResult.IsSuccess);
        var result = await handler.HandleAsync(cmdResult.Payload!);

        Assert.True(result.IsSuccess);

        var cmd = cmdResult.Payload!;

        var reloaded = await repo.GetAsync(id);
        Assert.NotNull(reloaded);
        Assert.Equal(desc, reloaded!.Description!.ToString());
    }

    [Fact]
    public async Task HandleAsync_Fails_When_Event_Not_Found()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var handler = new UpdateEventDescriptionHandler(repo, uow);

        var id = EventId.FromGuid(Guid.NewGuid());
        var ev = EventFactory.Init().WithId(id).Build();

        await repo.AddAsync(ev);

        string desc = "New description.";
        var cmdResult = UpdateEventDescriptionCommand.Create(Guid.NewGuid().ToString(), desc);
        Assert.True(cmdResult.IsSuccess);
        var result = await handler.HandleAsync(cmdResult.Payload!);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventNotFound);
    }
}