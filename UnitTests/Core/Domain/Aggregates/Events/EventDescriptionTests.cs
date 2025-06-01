using System;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;
using Xunit;

public class EventDescriptionTests
{
    [Fact]
    public void Create_WithValidDescription_ReturnsSuccess()
    {
        var description = "This is a valid event description.";
        var result = EventDescription.Create(description);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Payload);
        Assert.Equal(description, result.Payload.Value);
    }

    [Fact]
    public void Create_WithEmptyDescription_ReturnsSuccess()
    {
        var result = EventDescription.Create("");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Payload);
        Assert.Equal("", result.Payload.Value);
    }

    [Fact]
    public void Create_WithNull_ReturnsFailure()
    {
        var result = EventDescription.Create(null);

        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Equal(Error.EventDescriptionCannotBeNull, result.Errors[0]);
    }

    [Fact]
    public void Create_WithTooLongDescription_ReturnsFailure()
    {
        var longText = new string('x', 251);
        var result = EventDescription.Create(longText);

        Assert.True(result.IsFailure);
        Assert.Single(result.Errors);
        Assert.Equal(Error.EventDescriptionCannotExceed250Characters, result.Errors[0]);
    }

    [Fact]
    public void Equality_WorksForSameValue()
    {
        var d1 = EventDescription.Create("Same").Payload;
        var d2 = EventDescription.Create("Same").Payload;

        Assert.Equal(d1, d2);
    }
}