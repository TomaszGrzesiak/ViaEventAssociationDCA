using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Repositories;

public interface IEventRepository
{
    public Task<Result<VeaEvent>> AddAsync(VeaEvent @event);
    public Task<Result<VeaEvent>> GetViaEventByIdAsync(EventId eventId);
    public Task<Result<VeaEvent>> UpdateAsync(VeaEvent @event);
}