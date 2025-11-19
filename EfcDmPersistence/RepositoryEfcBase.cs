using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.Domain.Aggregates;
using ViaEventAssociation.Core.Domain.Common.Bases;

namespace EfcDmPersistence;

public abstract class RepositoryEfcBase<TAgg, TId>(DbContext context): IGenericRepository<TAgg, TId>
    where TAgg : AggregateRoot<TId>
    where  TId : Id<TId>
{
    public virtual async Task<TAgg?> GetAsync(TId id)
    {
        TAgg? root = await context.Set<TAgg>().FindAsync(id);
        // check for null. Throw exception, or return result
        return root!;
    }

    public virtual async Task RemoveAsync(TId id)
    {
        TAgg? root = await context.Set<TAgg>().FindAsync(id);
        // check for null. Throw exception, or return result
        context.Set<TAgg>().Remove(root!);
    }

    public virtual async Task AddAsync(TAgg aggregate)
        => await context.Set<TAgg>().AddAsync(aggregate);
}