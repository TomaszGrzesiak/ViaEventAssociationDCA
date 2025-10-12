using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry;

public interface ICommandDispatcher
{
    public Task<Result> DispatchAsync<TCommand>(TCommand command);
}