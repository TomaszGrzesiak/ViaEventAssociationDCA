using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Repositories;

public interface IEventRepository
{
    public Task<Result<Event>> AddAsync(Event @event);
    public Task<Result<Event>> GetViaEventByIdAsync(EventId eventId);
    public Task<Result<Event>> UpdateAsync(Event @event);
}