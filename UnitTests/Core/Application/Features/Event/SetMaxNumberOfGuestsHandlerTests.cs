using UnitTests.Fakes;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Application.Features.Event;

public class SetMaxNumberOfGuestsHandlerTests
{
    [Fact]
    public async Task HandleAsync_Updates_Description()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var handler = new SetMaxNumberOfGuestsHandler(repo, uow);

        var id = EventId.FromGuid(Guid.NewGuid());
        var ev = EventFactory.Init().WithId(id).Build();

        await repo.AddAsync(ev);

        var maxGuestNo = 15;
        var cmdResult = SetMaxNumberOfGuestsCommand.Create(id.ToString(), maxGuestNo);
        Assert.True(cmdResult.IsSuccess);
        var result = await handler.HandleAsync(cmdResult.Payload!);

        Assert.True(result.IsSuccess);

        var cmd = cmdResult.Payload!;

        Assert.Equal(maxGuestNo, ev.MaxGuestsNo.Value);
    }

    [Fact]
    public async Task HandleAsync_Fails_When_Event_Not_Found()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var handler = new SetMaxNumberOfGuestsHandler(repo, uow);

        var id = EventId.FromGuid(Guid.NewGuid());
        var ev = EventFactory.Init().WithId(id).Build();

        await repo.AddAsync(ev);

        var maxGuestNo = 15;
        var cmdResult = SetMaxNumberOfGuestsCommand.Create(Guid.NewGuid().ToString(), maxGuestNo);
        Assert.True(cmdResult.IsSuccess);
        var result = await handler.HandleAsync(cmdResult.Payload!);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventNotFound);
    }
}