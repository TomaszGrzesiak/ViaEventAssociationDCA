using ViaEventAssociation.Core.Domain.Common.Bases;

namespace ViaEventAssociation.Core.Domain.Aggregates.Guests;

public class GuestId(Guid value) : Id<GuestId>(value)
{
}