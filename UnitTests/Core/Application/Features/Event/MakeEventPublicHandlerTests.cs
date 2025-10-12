using UnitTests.Fakes;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Application.Features.Event;

public class MakeEventPublicHandlerTests
{
    [Fact]
    public async Task Handler_Makes_Event_Public_Test_Success()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var handler = new MakeEventPublicHandler(repo, uow);

        // Act
        var id = EventId.FromGuid(Guid.NewGuid());
        var ev = EventFactory.Init().WithId(id).WithVisibility(EventVisibility.Private).Build();
        await repo.AddAsync(ev);

        var cmd = MakeEventPublicCommand.Create(id.ToString());
        var result = await handler.HandleAsync(cmd.Payload!);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(ev!.Visibility, EventVisibility.Public);
    }

    [Fact]
    public async Task Handler_Makes_Event_Public_Test_Fails_When_Event_Not_Found()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var handler = new MakeEventPublicHandler(repo, uow);

        // Act
        var id = EventId.FromGuid(Guid.NewGuid());
        var ev = EventFactory.Init().WithId(id).WithVisibility(EventVisibility.Private).Build();
        await repo.AddAsync(ev);

        var newId = EventId.FromGuid(Guid.NewGuid());
        var cmd = MakeEventPublicCommand.Create(newId.ToString());
        var result = await handler.HandleAsync(cmd.Payload!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventNotFound);
        Assert.Equal(ev.Visibility, EventVisibility.Private);
    }
}