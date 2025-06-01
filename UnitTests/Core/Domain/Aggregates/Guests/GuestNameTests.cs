using System;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;
using Xunit;

public class GuestNameTests
{
    [Fact]
    public void Create_WithValidNames_ReturnsSuccessAndFormatsCorrectly()
    {
        var result = GuestName.Create("joHN", "doE");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Payload);
        Assert.Equal("John", result.Payload.FirstName);
        Assert.Equal("Doe", result.Payload.LastName);
    }

    [Theory]
    [InlineData(null, "Smith")]
    [InlineData("John", null)]
    [InlineData("", "Smith")]
    [InlineData("John", "")]
    [InlineData("A", "Smith")]
    [InlineData("John", "S")]
    [InlineData("ThisNameIsWayTooLongForValidation", "Smith")]
    [InlineData("John", "ThisLastNameIsWayTooLongToo")]
    [InlineData("J0hn", "Smith")]
    [InlineData("John", "Sm!th")]
    public void Create_WithInvalidNames_ReturnsFailure(string? first, string? last)
    {
        var result = GuestName.Create(first, last);

        Assert.True(result.IsFailure);
        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Equality_WorksForSameNames()
    {
        var result1 = GuestName.Create("Alice", "Wonderland").Payload;
        var result2 = GuestName.Create("alice", "WONDERLAND").Payload;

        Assert.Equal(result1, result2);
    }

    [Fact]
    public void ToString_ReturnsFormattedFullName()
    {
        var name = GuestName.Create("luke", "skywalker").Payload;

        Assert.Equal("Luke Skywalker", name.ToString());
    }
}