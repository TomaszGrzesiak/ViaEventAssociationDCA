using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Guests;

public interface IGuestRepository
{
    public Task AddAsync(Guest guest);

    public Task<Guest?> GetAsync(GuestId guestId);
}