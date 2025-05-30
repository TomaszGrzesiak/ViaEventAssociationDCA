using System;
using ViaEventAssociation.Core.Tools.OperationResult.Common.Bases;
using Xunit;

public class EventIdTests
{
    [Fact]
    public void FromString_WithValidGuid_ReturnsSuccess()
    {
        var guid = Guid.NewGuid().ToString();
        var result = EventId.FromString(guid);
        Assert.True(result.IsSuccess);
        Assert.Equal(guid, result.Payload.ToString());
    }

    [Fact]
    public void FromString_WithInvalidGuid_ReturnsFailure()
    {
        var result = EventId.FromString("not-a-guid");
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Equality_Works_ForSameGuid()
    {
        var guid = Guid.NewGuid();
        var id1 = EventId.FromGuid(guid);
        var id2 = EventId.FromGuid(guid);
        Assert.Equal(id1, id2);
    }
}