using ViaEventAssociation.Core.Domain.Aggregates.Events.Entities;

namespace UnitTests.Core.Domain.Aggregates.Events.Entities;

public class InvitationStatusTests
{
    [Theory]
    [InlineData(1, "Pending")]
    [InlineData(2, "Accepted")]
    [InlineData(3, "Declined")]
    public void FromId_WithValidId_ReturnsCorrectStatus(int id, string expectedName)
    {
        var result = InvitationStatus.FromId(id);
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedName, result.Payload!.Name);
    }

    [Fact]
    public void FromId_WithInvalidId_ReturnsFailure()
    {
        var result = InvitationStatus.FromId(99);
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("pending", 1)]
    [InlineData("ACCEPTED", 2)]
    [InlineData("Declined", 3)]
    public void FromName_WithValidName_IsCaseInsensitive(string name, int expectedId)
    {
        var result = InvitationStatus.FromName(name);
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedId, result.Payload.Id);
    }

    [Fact]
    public void FromName_WithInvalidName_ReturnsFailure()
    {
        var result = InvitationStatus.FromName("invalid");
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void ToString_ReturnsName()
    {
        Assert.Equal("Pending", InvitationStatus.Pending.ToString());
    }

    [Fact]
    public void Equals_ReturnsTrue_ForSameInstance()
    {
        var status1 = InvitationStatus.Accepted;
        var status2 = InvitationStatus.FromId(2).Payload!;
        Assert.Equal(status1, status2);
    }

    [Fact]
    public void Equals_ReturnsFalse_ForDifferentStatus()
    {
        var pending = InvitationStatus.Pending;
        var rejected = InvitationStatus.Declined;
        Assert.NotEqual(pending, rejected);
    }
}