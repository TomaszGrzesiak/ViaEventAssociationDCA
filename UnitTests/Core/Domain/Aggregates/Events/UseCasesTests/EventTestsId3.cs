using UnitTests.Helpers;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Core.Domain.Aggregates.Events.UseCasesTests;

public class EventTestsId3
{
    [Fact]
    public void Id3_S1_DescriptionValidAndStatusIsDraft_ShouldUpdate()
    {
        var eventId = EventId.FromGuid(Guid.NewGuid());
        var ev = VeaEvent.Create(eventId).Payload!;
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
        var eventId = EventId.FromGuid(Guid.NewGuid());
        var ev = VeaEvent.Create(eventId).Payload!;
        var emptyDescription = EventDescription.Create(input).Payload!;

        var result = ev.UpdateDescription(emptyDescription);

        Assert.True(result.IsSuccess);
        Assert.Equal("", ev.Description.Value);
    }

    [Fact]
    public void Id3_S3_UpdateDescriptionWhenReady_ShouldSetStatusToDraft()
    {
        var ev = EventFactory.Init().WithStatus(EventStatus.Ready).Build();

        var newDesc = EventDescription.Create("This is a valid description update.").Payload!;
        var result = ev.UpdateDescription(newDesc);

        Assert.True(result.IsSuccess);
        Assert.Equal(EventStatus.Draft, ev.Status);
    }

    [Fact]
    public void Id3_F1_DescriptionTooLong_ShouldFail()
    {
        var eventId = EventId.FromGuid(Guid.NewGuid());
        var ev = VeaEvent.Create(eventId).Payload!;
        var tooLongText = new string('a', 251);
        var result = EventDescription.Create(tooLongText);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.EventDescriptionCannotExceed250Characters);
    }

    [Fact]
    public void Id3_F2_DescriptionUpdateWhenCancelled_ShouldFail()
    {
        var eventId = EventId.FromGuid(Guid.NewGuid());
        var ev = VeaEvent.Create(eventId).Payload!;
        ev.Cancel();

        var desc = EventDescription.Create("Some text").Payload!;
        var result = ev.UpdateDescription(desc);

        Assert.True(result.IsFailure);
        Assert.Contains(result.Errors, e => e == Error.ActiveOrCanceledEventCannotBeModified);
    }

    [Fact]
    public void Id3_F3_DescriptionUpdateWhenActive_ShouldFail()
    {
        var ev = EventFactory.Init().WithStatus(EventStatus.Active).Build();

        var desc = EventDescription.Create("Another description").Payload!;
        var result = ev.UpdateDescription(desc);

        Assert.False(result.IsSuccess);
        Assert.Contains(result.Errors, e => e == Error.ActiveOrCanceledEventCannotBeModified);
    }
}