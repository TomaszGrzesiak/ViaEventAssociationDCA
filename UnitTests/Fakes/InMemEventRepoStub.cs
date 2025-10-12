using System.Collections.Concurrent;
using ViaEventAssociation.Core.Domain.Aggregates.Events;

namespace UnitTests.Fakes;

public class InMemEventRepoStub : IEventRepository
{
    // ConcurrentDictionary works like Map in TS, and is Thread-safe in case multiple threads want to read/write on it.
    // NOTE: This assumes EventId has value equality (record/struct with proper GetHashCode).
    private readonly ConcurrentDictionary<EventId, VeaEvent> _store = new();

    public Task AddAsync(VeaEvent aggregate)
    {
        _store[aggregate.Id] = aggregate;
        return Task.CompletedTask;
    }


    public Task<VeaEvent?> GetAsync(EventId id)
        => Task.FromResult(_store.GetValueOrDefault(id)); // GetValueOrDefault(key) returns the value if present, or default (null for classes) if not.
}