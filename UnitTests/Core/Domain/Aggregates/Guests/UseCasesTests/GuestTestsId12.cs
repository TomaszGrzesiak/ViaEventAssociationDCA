using UnitTests.Helpers;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Domain.Aggregates.Guests.UseCasesTests;

public class EventTestsId12
{
    [Fact]
    public void Id12_S1_RemovesParticipation_WhenGuestIsParticipating()
    {
        var guest = GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithGuest(guest.Id)
            .Build();

        Assert.Contains(veaEvent.GuestList, g => g == guest.Id);

        var result = veaEvent.CancelParticipation(guest.Id);
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(veaEvent.GuestList, g => g == guest.Id);
    }

    [Fact]
    public void Id12_S2_Noop_WhenGuestIsNotParticipating()
    {
        var guest = GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .Build();

        Assert.DoesNotContain(veaEvent.GuestList, g => g == guest.Id);
        var result = veaEvent.CancelParticipation(guest.Id);

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(veaEvent.GuestList, g => g == guest.Id);
    }

    [Fact]
    public void Id12_F1_Failure_WhenEventStartedAlready()
    {
        var pastStart = EventTimeRange.Default().StartTime.AddDays(-2);
        var pastDateTime = EventTimeRange.Create(pastStart, pastStart.AddHours(2)).Payload!;

        var guest = GuestFactory.Init().Build();
        var veaEvent = EventFactory.Init()
            .WithGuest(guest.Id)
            .WithTimeRange(pastDateTime)
            .Build();

        var result = veaEvent.CancelParticipation(guest.Id);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.ActiveOrCanceledEventCannotBeModified);
    }
}