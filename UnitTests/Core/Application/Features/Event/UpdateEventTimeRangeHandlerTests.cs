using UnitTests.Fakes;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Application.Features;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Application.Features.Event;

public class UpdateEventTimeRangeHandlerTests
{
    private static readonly ISystemTime FakeSystemTime = new FakeSystemTime(new DateTime(2023, 8, 10, 12, 0, 0));

    [Fact]
    public async Task HandleAsync_Updates_Event_TimeRange()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var handler = new UpdateEventTimeRangeHandler(repo, uow, FakeSystemTime);

        var id = EventId.FromGuid(Guid.NewGuid());
        var ev = EventFactory.Init().WithId(id).Build();
        await repo.AddAsync(ev);

        var startTime = "2023-08-25 19:00";
        var endTime = "2023-08-25 23:59";
        var cmdResult = UpdateEventTimeRangeCommand.Create(id.ToString(), startTime, endTime);

        // Act
        var result = await handler.HandleAsync(cmdResult.Payload!);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task HandleAsync_Fails_When_Event_Not_Found()
    {
        // Arrange
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var handler = new UpdateEventTimeRangeHandler(repo, uow, FakeSystemTime);

        var id = EventId.FromGuid(Guid.NewGuid());
        var ev = EventFactory.Init().WithId(id).Build();
        await repo.AddAsync(ev);

        var startTime = "2023-08-25 19:00";
        var endTime = "2023-08-25 23:59";
        var newGuid = Guid.NewGuid();
        var cmdResult = UpdateEventTimeRangeCommand.Create(newGuid.ToString(), startTime, endTime);

        // Act
        var result = await handler.HandleAsync(cmdResult.Payload!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventNotFound);
    }
}