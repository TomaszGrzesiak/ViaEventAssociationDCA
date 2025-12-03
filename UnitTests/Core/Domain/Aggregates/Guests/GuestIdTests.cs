using ViaEventAssociation.Core.Domain.Aggregates.Guests;

namespace UnitTests.Core.Domain.Aggregates.Guests;

public class GuestIdTests
{
    [Fact]
    public void FromString_WithValidGuid_ReturnsSuccess()
    {
        var guid = Guid.NewGuid().ToString();
        var result = GuestId.FromString(guid);
        Assert.True(result.IsSuccess);
        Assert.Equal(guid, result.Payload!.ToString());
    }

    [Fact]
    public void FromString_WithInvalidGuid_ReturnsFailure()
    {
        var result = GuestId.FromString("not-a-guid");
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Equality_Works_ForSameGuid()
    {
        var guid = Guid.NewGuid();
        var id1 = GuestId.FromGuid(guid);
        var id2 = GuestId.FromGuid(guid);
        Assert.Equal(id1, id2);
    }
}