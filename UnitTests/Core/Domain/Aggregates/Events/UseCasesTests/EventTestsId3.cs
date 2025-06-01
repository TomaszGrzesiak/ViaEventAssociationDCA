using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Domain.Aggregates.Events.UseCasesTests;

public class EventTestsId3
{
    [Fact]
    public void Id3_S1_DescriptionValidAndStatusIsDraft_ShouldUpdate()
    {
        var ev = VeaEvent.Create().Payload!;
        var descriptionTextShortened = "Nullam tempor lacus nisl, eget tempus quam maximus malesuada. " +
                                       "Morbi faucibus sed neque vitae euismod. Vestibulum non purus vel justo ornare vulputate. " +
                                       "In a interdum enim. Maecenas sed sodales elit, sit amet venenatis orci.";

        var desc = EventDescription.Create(descriptionTextShortened).Payload!;
        var result = ev.UpdateDescription(desc);

        Assert.True(result.IsSuccess);
        Assert.Equal(descriptionTextShortened, ev.Description.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Id3_S2_UpdateDescriptionToEmpty_ShouldSucceed(string input)
    {
        var ev = VeaEvent.Create().Payload!;
        var emptyDescription = EventDescription.Create(input).Payload!;

        var result = ev.UpdateDescription(emptyDescription);

        Assert.True(result.IsSuccess);
        Assert.Equal("", ev.Description.Value);
    }

    [Fact]
    public void Id3_S3_UpdateDescriptionWhenReady_ShouldSetStatusToDraft()
    {
        var ev = VeaEvent.Create().Payload!;

        ev.UpdateTimeRange(EventTimeRange.Default());
        ev.UpdateMaxGuests(MaxGuests.Default());
        ev.UpdateVisibility(EventVisibility.Public);
        ev.Ready();
        Assert.Equal(EventStatus.Ready, ev.Status);

        var newDesc = EventDescription.Create("This is a valid description update.").Payload!;
        var result = ev.UpdateDescription(newDesc);

        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Draft, ev.Status);
    }

    [Fact]
    public void Id3_F1_DescriptionTooLong_ShouldFail()
    {
        var ev = VeaEvent.Create().Payload!;
        var tooLongText = new string('a', 251);
        var result = EventDescription.Create(tooLongText);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventDescriptionCannotExceed250Characters);
    }

    [Fact]
    public void Id3_F2_DescriptionUpdateWhenCancelled_ShouldFail()
    {
        var ev = VeaEvent.Create().Payload!;
        ev.Cancel();

        var desc = EventDescription.Create("Some text").Payload!;
        var result = ev.UpdateDescription(desc);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.ActiveOrCanceledEventCannotBeModified);
    }

    [Fact]
    public void Id3_F3_DescriptionUpdateWhenActive_ShouldFail()
    {
        var ev = VeaEvent.Create().Payload!;
        ev.UpdateTimeRange(EventTimeRange.Create(DateTime.Now, DateTime.Now.AddHours(2)).Payload!);
        ev.UpdateMaxGuests(MaxGuests.Create(10).Payload!);
        ev.UpdateVisibility(EventVisibility.Public);
        ev.Ready();
        ev.Activate();

        var desc = EventDescription.Create("Another description").Payload!;
        var result = ev.UpdateDescription(desc);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.ActiveOrCanceledEventCannotBeModified);
    }
}