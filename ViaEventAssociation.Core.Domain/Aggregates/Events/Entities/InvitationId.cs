using ViaEventAssociation.Core.Domain.Common.Bases;

namespace ViaEventAssociation.Core.Domain.Aggregates.Events.Entities;

// Assuming Id<T> exposes a Guid Value
public class InvitationId : Id<InvitationId>
{
    // For EF Core materialization
    private InvitationId() : base(Guid.Empty) { }

    public InvitationId(Guid value) : base(value) { }
}