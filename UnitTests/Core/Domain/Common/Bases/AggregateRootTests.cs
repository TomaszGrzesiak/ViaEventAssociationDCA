using ViaEventAssociation.Core.Domain.Common.Bases;

namespace UnitTests.Core.Domain.Common.Bases;

public class DummyAggregateId(Guid value) : Id<DummyAggregateId>(value);

public class DummyAggregate(DummyAggregateId id, string name) : AggregateRoot<DummyAggregateId>(id)
{
    public string Name { get; } = name;
}

public class AggregateRootTests
{
    [Fact]
    public void AggregateRoots_With_Same_Id_Should_Be_Equal()
    {
        var id = new DummyAggregateId(Guid.NewGuid());
        var aggregate1 = new DummyAggregate(id, "Test A");
        var aggregate2 = new DummyAggregate(id, "Test B");

        Assert.Equal(aggregate1, aggregate2);
    }

    [Fact]
    public void AggregateRoots_With_Different_Ids_Should_Not_Be_Equal()
    {
        var aggregate1 = new DummyAggregate(new DummyAggregateId(Guid.NewGuid()), "A");
        var aggregate2 = new DummyAggregate(new DummyAggregateId(Guid.NewGuid()), "B");

        Assert.NotEqual(aggregate1, aggregate2);
    }
}