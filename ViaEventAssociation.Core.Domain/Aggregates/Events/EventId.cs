using ViaEventAssociation.Core.Domain.Common.Bases;

namespace ViaEventAssociation.Core.Domain.Aggregates.Events;

public class EventId : Id<EventId>
{
    protected EventId(Guid value) : base(value)
    {
    }
}