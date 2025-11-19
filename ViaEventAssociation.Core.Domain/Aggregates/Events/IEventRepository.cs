using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Domain.Aggregates.Events;

public interface IEventRepository: IGenericRepository<VeaEvent, EventId>
{

}