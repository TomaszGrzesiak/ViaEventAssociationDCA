using ViaEventAssociation.Core.Domain.Contracts;

namespace UnitTests.Fakes;

public class FakeSystemTime(DateTime? fixedTime) : ISystemTime
{
    public DateTime Now() => fixedTime ??  new DateTime(2023, 10, 30, 12, 0, 0);
}