using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Domain.Aggregates.Events.UseCasesTests;

public class EventTestsId5
{
    private VeaEvent CreateBaseEvent(string statusString)
    {
        var title = EventTitle.Create("Test Event").Payload!;
        var status = EventStatus.FromName(statusString);
        Assert.True(status.IsSuccess);
        return VeaEvent.Create(title, status.Payload!).Payload!;
    }

    [Theory]
    [InlineData("Draft")]
    [InlineData("Ready")]
    [InlineData("Active")]
    public void Id5_S1_EventCanBeMadePublic(string statusString)
    {
        var ev = CreateBaseEvent(statusString);
        var result = ev.UpdateVisibility(EventVisibility.Public);

        Assert.True(result.IsSuccess);
        Assert.Equal(EventVisibility.Public, ev.Visibility);
        Assert.Equal(statusString, ev.Status.Name);
    }

    [Fact]
    public void Id5_F1_Cancelled_EventCannotBeMadePublic()
    {
        var ev = CreateBaseEvent("Draft");
        ev.Cancel();

        var result = ev.UpdateVisibility(EventVisibility.Public);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventAlreadyCancelled);
    }
}