using ViaEventAssociation.Core.Domain.Aggregates.Events.Entities;

namespace UnitTests.Core.Domain.Aggregates.Events.Entities;

public class InvitationIdTests
{
    [Fact]
    public void FromString_WithValidGuid_ReturnsSuccess()
    {
        var guid = Guid.NewGuid().ToString();
        var result = InvitationId.FromString(guid);
        Assert.True(result.IsSuccess);
        Assert.Equal(guid, result.Payload!.ToString());
    }

    [Fact]
    public void FromString_WithInvalidGuid_ReturnsFailure()
    {
        var result = InvitationId.FromString("not-a-guid");
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Equality_Works_ForSameGuid()
    {
        var guid = Guid.NewGuid();
        var id1 = InvitationId.FromGuid(guid);
        var id2 = InvitationId.FromGuid(guid);
        Assert.Equal(id1, id2);
    }
}