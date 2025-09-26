using ViaEventAssociation.Core.Domain.Contracts;

namespace UnitTests.Fakes;

public class FakeSystemTime(DateTime fixedTime) : ISystemTime
{
    public DateTime Now() => fixedTime;
}