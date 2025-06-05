using System.Collections.ObjectModel;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Repositories;

public class DummyEventRepository : IEventRepository
{
    private readonly Collection<Event> _events = new Collection<Event>();

    public Task<Result<Event>> AddAsync(Event @event)
    {
        _events.Add(@event);

        return Task.FromResult(Result<Event>.Success(@event));
    }

    public Task<Result<Event>> GetViaEventByIdAsync(EventId eventId)
    {
        var @event = _events.FirstOrDefault(e => e.Id.Equals(eventId));
        return Task.FromResult(Result<Event>.Success(@event));
    }

    public Task<Result<Event>> UpdateAsync(Event @event)
    {
        var existingEvent = _events.FirstOrDefault(e => e.Id == @event.Id);

        existingEvent = @event;

        return Task.FromResult(Result<Event>.Success(existingEvent));
    }
}