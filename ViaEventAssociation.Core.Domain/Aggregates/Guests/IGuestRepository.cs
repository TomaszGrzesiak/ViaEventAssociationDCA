using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Guests;

public interface IGuestRepository
{
    public Task<Result<Guest>> AddAsync(Guest guest);

    public Task<Result<Guest>> GetGuestByIdAsync(GuestId guestId);
}