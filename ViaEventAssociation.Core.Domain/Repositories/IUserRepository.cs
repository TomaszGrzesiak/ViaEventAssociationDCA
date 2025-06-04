using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Repositories;

public interface IUserRepository
{
    public Task<Result<Guest>> AddAsync(Guest guest);
    public Task<Result<Guest>> GetUserByIdAsync(GuestId guestId);
}