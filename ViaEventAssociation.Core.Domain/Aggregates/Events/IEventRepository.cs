using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Events;

public interface IEventRepository
{
    public Task AddAsync(VeaEvent veaEvent);
    public Task<VeaEvent?> GetAsync(EventId eventId);
}