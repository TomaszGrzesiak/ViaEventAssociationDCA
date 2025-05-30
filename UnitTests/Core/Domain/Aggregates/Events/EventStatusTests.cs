using ViaEventAssociation.Core.Domain.Aggregates.Events;

namespace UnitTests.Core.Domain.Aggregates.Events;

public class EventStatusTests
{
    [Theory]
    [InlineData(1, "Draft")]
    [InlineData(2, "Active")]
    [InlineData(3, "Cancelled")]
    [InlineData(4, "Ready")]
    public void FromId_WithValidId_ReturnsCorrectStatus(int id, string expectedName)
    {
        var result = EventStatus.FromId(id);
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedName, result.Payload!.Name);
    }

    [Fact]
    public void FromId_WithInvalidId_ReturnsFailure()
    {
        var result = EventStatus.FromId(99);
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("draft", 1)]
    [InlineData("ACTIVE", 2)]
    [InlineData("Cancelled", 3)]
    [InlineData("ready", 4)]
    public void FromName_WithValidName_IsCaseInsensitive(string name, int expectedId)
    {
        var result = EventStatus.FromName(name);
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedId, result.Payload!.Id);
    }

    [Fact]
    public void FromName_WithInvalidName_ReturnsFailure()
    {
        var result = EventStatus.FromName("invalid");
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void ToString_ReturnsName()
    {
        Assert.Equal("Ready", EventStatus.Ready.ToString());
    }

    [Fact]
    public void Equals_ReturnsTrue_ForSameInstance()
    {
        var status1 = EventStatus.Cancelled;
        var status2 = EventStatus.FromId(3).Payload!;
        Assert.Equal(status1, status2);
    }

    [Fact]
    public void Equals_ReturnsFalse_ForDifferentStatus()
    {
        var draft = EventStatus.Draft;
        var active = EventStatus.Active;
        Assert.NotEqual(draft, active);
    }
}