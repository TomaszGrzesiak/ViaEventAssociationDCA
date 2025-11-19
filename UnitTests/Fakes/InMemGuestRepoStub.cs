using System.Collections.Concurrent;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;

namespace UnitTests.Fakes;

public class InMemGuestRepoStub : IGuestRepository
{
    // ConcurrentDictionary works like Map in TS, and is Thread-safe in case multiple threads want to read/write on it.
    // NOTE: This assumes EventId has value equality (record/struct with proper GetHashCode).
    private readonly ConcurrentDictionary<GuestId, Guest> _store = new();

    public Task RemoveAsync(GuestId id)
    {
        _store.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    public Task AddAsync(Guest guest)
    {
        _store[guest.Id!] = guest;
        return Task.CompletedTask;
    }

    public Task<Guest?> GetAsync(GuestId id)
        => Task.FromResult(_store.GetValueOrDefault(id)); // GetValueOrDefault(key) returns the value if present, or default (null for classes) if not.
}