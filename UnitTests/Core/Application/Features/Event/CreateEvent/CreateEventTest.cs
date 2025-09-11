using Application.Features.Event;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;

namespace UnitTests.Core.Application.Features.Event.CreateEvent;

public class CreateEventTest
{
    [Fact]
    public async Task HandleAsync_CreatesEvent_AndSaves()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        var handler = new CreateEventHandler(repo, uow);

        var guid = Guid.NewGuid();
        var cmdResult = CreateEventCommand.Create(guid.ToString()); // always success by design
        Assert.True(cmdResult.IsSuccess);
        var cmd = cmdResult.Payload!;

        var result = await handler.HandleAsync(cmd);

        Assert.True(result.IsSuccess);
        var newEvent = await repo.GetAsync(EventId.FromGuid(guid));
        Assert.NotNull(newEvent);
        Assert.Equal(newEvent.Id, EventId.FromGuid(guid));
        // Optional: assert repo now contains exactly one event, with default values.
        // (Depends on your InMem repo API)
    }

    // No failure test: command has no inputs; domain sets defaults and cannot fail here.
}