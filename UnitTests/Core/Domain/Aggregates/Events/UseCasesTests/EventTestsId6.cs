using System.Reflection.Emit;
using UnitTests.Helpers;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Domain.Aggregates.Events.UseCasesTests;

public class EventTestsId6
{
    [Theory]
    [InlineData("Draft")]
    [InlineData("Ready")]
    public void Id6_S1_AlreadyPrivate_Draft_ShouldRemainUnchanged(string statusString)
    {
        var ev = EventFactory.Init()
            .WithStatus(EventStatus.FromName(statusString).Payload!)
            .WithVisibility(EventVisibility.Private)
            .Build();

        Assert.Equal(EventVisibility.Private, ev.Visibility);
        Assert.Equal(statusString, ev.Status.Name);
    }

    [Theory]
    [InlineData("Draft")]
    [InlineData("Ready")]
    public void Id6_S2_PublicToPrivate_Draft_StatusStaysOrBecomesDraft(string statusString)
    {
        var ev = EventFactory.Init()
            .WithStatus(EventStatus.FromName(statusString).Payload!)
            .WithVisibility(EventVisibility.Public)
            .Build();

        var result = ev.UpdateVisibility(EventVisibility.Private);
        Assert.True(result.IsSuccess);

        Assert.Equal(EventVisibility.Private, ev.Visibility);
        Assert.Equal(EventStatus.Draft, ev.Status);
    }
}