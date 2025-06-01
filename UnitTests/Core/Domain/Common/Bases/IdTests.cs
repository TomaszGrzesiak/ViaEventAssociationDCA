using ViaEventAssociation.Core.Domain.Common.Bases;

namespace UnitTests.Core.Domain.Common.Bases;

public class SampleId(Guid value) : Id<SampleId>(value);

public class IdTests
{
    [Fact]
    public void Ids_With_Same_Guid_Should_Be_Equal()
    {
        var guid = Guid.NewGuid();
        var id1 = new SampleId(guid);
        var id2 = new SampleId(guid);

        Assert.Equal(id1, id2);
        Assert.True(id1 == id2);
    }

    [Fact]
    public void Ids_With_Different_Guid_Should_Not_Be_Equal()
    {
        var id1 = new SampleId(Guid.NewGuid());
        var id2 = new SampleId(Guid.NewGuid());

        Assert.NotEqual(id1, id2);
        Assert.True(id1 != id2);
    }

    [Fact]
    public void Id_ToString_Should_Return_Guid_String()
    {
        var guid = Guid.NewGuid();
        var id = new SampleId(guid);

        Assert.Equal(guid.ToString(), id.ToString());
    }
}