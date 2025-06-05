namespace ViaEventAssociation.Core.Domain.Aggregates.Events;

public interface IEventRepository
{
    // TODO: See Assignment 5
}

using System.Threading.Tasks;

namespace ViaEventAssociation.Core.Domain.Repositories;

public interface IEventRepository
{
    Task AddAsync(VeaEvent veaEvent);
    Task<VeaEvent?> GetByIdAsync(EventId id);
}