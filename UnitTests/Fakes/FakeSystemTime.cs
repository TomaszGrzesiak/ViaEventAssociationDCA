using ViaEventAssociation.Core.Domain.Contracts;

namespace UnitTests.Fakes;

public sealed class FakeSystemTime : ISystemTime
{
    private DateTime _now;

    // Used by DI in integration tests
    public FakeSystemTime()
        : this(DateTime.UtcNow) // or some default
    {
    }

    // Used explicitly in unit tests
    public FakeSystemTime(DateTime now)
    {
        _now = now;
    }

    public DateTime Now() => _now;

    // Optional helper for tests that want to change time mid-test
    public void Set(DateTime now) => _now = now;
}
