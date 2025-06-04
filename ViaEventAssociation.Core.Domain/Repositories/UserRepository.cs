using System.Collections.ObjectModel;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Repositories;

public class UserRepository : IUserRepository
{
    private Collection<Guest> _guest = new Collection<Guest>();

    public Task<Result<Guest>> AddAsync(Guest guest)
    {
        _guest.Add(guest);
        return Task.FromResult(Result<Guest>.Success(guest));
    }

    public Task<Result<Guest>> GetUserByIdAsync(GuestId guestId)
    {
        var guest = _guest.FirstOrDefault(u => u.Id.Equals(guestId));
        return Task.FromResult(Result<Guest>.Success(guest));
    }
}