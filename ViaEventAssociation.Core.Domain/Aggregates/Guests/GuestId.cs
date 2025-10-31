using ViaEventAssociation.Core.Domain.Common.Bases;

namespace ViaEventAssociation.Core.Domain.Aggregates.Guests;

public class GuestId : Id<GuestId>
{
    private GuestId() {}
    protected GuestId(Guid value) : base(value)
    {
    }
}