using ViaEventAssociation.Core.Domain.Common.Bases;

namespace UnitTests.Core.Domain.Common.Bases;

public class DummyVo(int a, string b) : ValueObject
{
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return a;
        yield return b;
    }

    public override string ToString()
    {
        return $"{a}-{b}";
    }
}

public class ValueObjectTests
{
    [Fact]
    public void ValueObjects_With_Same_Components_Should_Be_Equal()
    {
        var vo1 = new DummyVo(1, "test");
        var vo2 = new DummyVo(1, "test");

        Assert.Equal(vo1, vo2);
        Assert.True(vo1 == vo2);
        Assert.False(vo1 != vo2);
    }

    [Fact]
    public void ValueObjects_With_Different_Components_Should_Not_Be_Equal()
    {
        var vo1 = new DummyVo(1, "a");
        var vo2 = new DummyVo(2, "b");

        Assert.NotEqual(vo1, vo2);
        Assert.True(vo1 != vo2);
        Assert.False(vo1 == vo2);
    }
}