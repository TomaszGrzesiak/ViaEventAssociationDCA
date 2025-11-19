using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Guests;

public interface IGuestRepository : IGenericRepository<Guest, GuestId>
{
}