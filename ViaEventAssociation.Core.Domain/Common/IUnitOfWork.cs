namespace ViaEventAssociation.Core.Domain.Common;

public interface IUnitOfWork
{
    public Task SaveChangesAsync();
}