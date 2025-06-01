using System;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using Xunit;

public class EventTitleTests
{
    [Fact]
    public void Create_WithValidTitle_ReturnsSuccess()
    {
        var result = EventTitle.Create("My Cool VeaEvent");
        Assert.True(result.IsSuccess);
        Assert.Equal("My Cool VeaEvent", result.Payload.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("     ")]
    [InlineData(null)]
    public void Create_WithEmptyOrWhitespace_ReturnsFailure(string input)
    {
        var result = EventTitle.Create(input);
        Assert.True(result.IsFailure);
        Assert.True(result.Errors.Count > 0, "Should contain at least 1 error.");
    }

    [Fact]
    public void Create_WithTooLongTitle_ReturnsFailure()
    {
        string longTitle = new string('a', 101);
        var result = EventTitle.Create(longTitle);
        Assert.True(result.IsFailure);
        Assert.True(result.Errors.Count > 0, "Should contain at least 1 error.");
    }

    [Fact]
    public void Equality_WorksForSameTitle()
    {
        var result1 = EventTitle.Create("Title");
        var result2 = EventTitle.Create("Title");

        Assert.Equal(result1.Payload, result2.Payload);
    }
}