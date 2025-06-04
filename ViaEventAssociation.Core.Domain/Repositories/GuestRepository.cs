using System.Collections.ObjectModel;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Repositories;

public class GuestRepository : IGuestRepository
{
    private Collection<Guest> _guests = new Collection<Guest>();
    
    public Task<Result<Guest>> AddAsync(Guest guest)
    {
        _guests.Add(guest);
        return Task.FromResult(Result<Guest>.Success(guest));
    }

    public Task<Result<Guest>> GetGuestByIdAsync(GuestId guestId)
    {
        var guest = _guests.FirstOrDefault(g => g.Id.Equals(guestId));
        return Task.FromResult(Result<Guest>.Success(guest));
    }
}