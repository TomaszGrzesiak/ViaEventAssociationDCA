using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.Domain.Common;

namespace EfcDmPersistence.UnitOfWork;

public class SqliteUnitOfWork(DbContext context) : IUnitOfWork
{
    public Task SaveChangesAsync() => context.SaveChangesAsync();
}
