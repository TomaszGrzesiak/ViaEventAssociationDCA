using ViaEventAssociation.Core.Domain.Common.Bases;

namespace UnitTests.Core.Domain.Common.Bases;

public class DummyVO : ValueObject
{
    private readonly int _a;
    private readonly string _b;

    public DummyVO(int a, string b)
    {
        _a = a;
        _b = b;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _a;
        yield return _b;
    }

    public override string ToString()
    {
        return $"{_a}-{_b}";
    }
}

public class ValueObjectTests
{
    [Fact]
    public void ValueObjects_With_Same_Components_Should_Be_Equal()
    {
        var vo1 = new DummyVO(1, "test");
        var vo2 = new DummyVO(1, "test");

        Assert.Equal(vo1, vo2);
        Assert.True(vo1 == vo2);
        Assert.False(vo1 != vo2);
    }

    [Fact]
    public void ValueObjects_With_Different_Components_Should_Not_Be_Equal()
    {
        var vo1 = new DummyVO(1, "a");
        var vo2 = new DummyVO(2, "b");

        Assert.NotEqual(vo1, vo2);
        Assert.True(vo1 != vo2);
        Assert.False(vo1 == vo2);
    }
}