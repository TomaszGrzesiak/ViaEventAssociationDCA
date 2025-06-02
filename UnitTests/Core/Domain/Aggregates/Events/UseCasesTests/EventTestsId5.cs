using UnitTests.Helpers;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Domain.Aggregates.Events.UseCasesTests;

public class EventTestsId5
{
    [Theory]
    [InlineData("Draft")]
    [InlineData("Ready")]
    [InlineData("Active")]
    public void Id5_S1_EventCanBeMadePublic(string statusString)
    {
        var ev = EventFactory.Init().WithStatus(EventStatus.FromName(statusString).Payload!).WithTitle("Test Event.").Build();
        var result = ev.UpdateVisibility(EventVisibility.Public);

        Assert.True(result.IsSuccess);
        Assert.Equal(EventVisibility.Public, ev.Visibility);
        Assert.Equal(statusString, ev.Status.Name);
    }

    [Fact]
    public void Id5_F1_Cancelled_EventCannotBeMadePublic()
    {
        var ev = EventFactory.Init().WithStatus(EventStatus.Draft).WithTitle("Test Event.").Build();
        ev.Cancel();

        var result = ev.UpdateVisibility(EventVisibility.Public);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventAlreadyCancelled);
    }
}