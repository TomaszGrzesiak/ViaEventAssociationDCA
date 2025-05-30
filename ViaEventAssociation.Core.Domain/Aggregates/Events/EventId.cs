using ViaEventAssociation.Core.Domain.Common.Bases;

namespace ViaEventAssociation.Core.Domain.Aggregates.Events;

public class EventId(Guid value) : Id<EventId>(value)
{
}