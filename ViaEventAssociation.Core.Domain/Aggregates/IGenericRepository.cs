namespace ViaEventAssociation.Core.Domain.Aggregates;

public interface IGenericRepository<Taggr, Tid>
{
    public Task<Taggr?> GetAsync(Tid id);
    public Task RemoveAsync(Tid id);
    public Task AddAsync(Taggr a);
}