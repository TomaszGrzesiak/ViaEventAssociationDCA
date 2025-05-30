using ViaEventAssociation.Core.Domain.Common.Bases;

namespace UnitTests.Core.Domain.Common.Bases;

public class DummyId : Id<DummyId>
{
    public DummyId(Guid value) : base(value)
    {
    }
}

public class DummyEntity : Entity<DummyId>
{
    public DummyEntity(DummyId id) : base(id)
    {
    }
}

public class EntityTests
{
    [Fact]
    public void Entities_With_Same_Id_Should_Be_Equal()
    {
        var id = new DummyId(Guid.NewGuid());
        var entity1 = new DummyEntity(id);
        var entity2 = new DummyEntity(id);

        Assert.Equal(entity1, entity2);
        Assert.True(entity1 == entity2);
        Assert.False(entity1 != entity2);
    }

    [Fact]
    public void Entities_With_Different_Ids_Should_Not_Be_Equal()
    {
        var entity1 = new DummyEntity(new DummyId(Guid.NewGuid()));
        var entity2 = new DummyEntity(new DummyId(Guid.NewGuid()));

        Assert.NotEqual(entity1, entity2);
        Assert.True(entity1 != entity2);
        Assert.False(entity1 == entity2);
    }
}