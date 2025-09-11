using ViaEventAssociation.Core.Domain.Common;

namespace UnitTests.Fakes;

public class FakeUoW : IUnitOfWork
{
    public Task SaveChangesAsync() => Task.CompletedTask;
}