using ViaEventAssociation.Core.Tools.OperationResult;

namespace Application.AppEntry;

public interface ICommandHandler<TCommand>
{
    Task<Result> HandleAsync(TCommand command);
}