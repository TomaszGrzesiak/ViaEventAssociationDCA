using System;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using Xunit;

public class MaxGuestsTests
{
    [Fact]
    public void Create_WithValidValue_ReturnsSuccess()
    {
        var result = MaxGuests.Create(5);
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Payload.Value);
    }

    [Fact]
    public void Create_With4OrLess_ReturnsFailure()
    {
        var resultZero = MaxGuests.Create(4);
        var resultNegative = MaxGuests.Create(-3);

        Assert.True(resultZero.IsFailure);
        Assert.True(resultNegative.IsFailure);
    }

    [Fact]
    public void Equality_WorksForSameValue()
    {
        var result1 = MaxGuests.Create(6);
        var result2 = MaxGuests.Create(6);

        Assert.Equal(result1.Payload, result2.Payload);
    }
}