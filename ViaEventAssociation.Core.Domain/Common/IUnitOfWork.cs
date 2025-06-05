namespace ViaEventAssociation.Core.Domain.Common;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}