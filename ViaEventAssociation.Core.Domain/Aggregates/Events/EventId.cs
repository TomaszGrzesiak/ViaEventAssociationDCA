using ViaEventAssociation.Core.Domain.Common.Bases;

namespace ViaEventAssociation.Core.Tools.OperationResult.Common.Bases;

public class EventId(Guid value) : Id<EventId>(value)
{
}