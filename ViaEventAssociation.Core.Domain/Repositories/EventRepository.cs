using System.Collections.ObjectModel;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Repositories;

public class EventRepository : IEventRepository
{
    private readonly Collection<VeaEvent> _events = new Collection<VeaEvent>();
    
    public Task<Result<VeaEvent>> AddAsync(VeaEvent @event)
    {
        _events.Add(@event);

        return Task.FromResult(Result<VeaEvent>.Success(@event));
    }
    
    Task<Result<VeaEvent>> IEventRepository.GetViaEventByIdAsync(EventId eventId)
    {
        throw new NotImplementedException();
    }
    
    public Task<Result<VeaEvent>> GetViaEventByIdAsync(EventId eventId)
    {
        var @event = _events.FirstOrDefault(e => e.Id.Equals(eventId));
        return Task.FromResult(Result<VeaEvent>.Success(@event));
    }

    public Task<Result<VeaEvent>> UpdateAsync(VeaEvent @event)
    {
        var existingEvent = _events.FirstOrDefault(e => e.Id == @event.Id);

        existingEvent = @event;

        return Task.FromResult(Result<VeaEvent>.Success(existingEvent));
    }
}