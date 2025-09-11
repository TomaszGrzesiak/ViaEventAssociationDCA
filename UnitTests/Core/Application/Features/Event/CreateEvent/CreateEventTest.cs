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

        // S1
        Assert.Equal(newEvent.Status, EventStatus.Draft);

        // S2
        Assert.Equal("Working title.", newEvent.Title?.ToString());

        // S3
        Assert.Equal("", newEvent.Description?.ToString());

        // S4
        Assert.Equal(newEvent.Visibility, EventVisibility.Private);
    }
}