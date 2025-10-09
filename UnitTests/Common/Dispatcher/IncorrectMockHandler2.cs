using Application.AppEntry;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Common.Dispatcher;

public class IncorrectMockHandler2 : ICommandHandler<None>
{
    public int HandleCallCount { get; private set; }

    public Task<Result> HandleAsync(None command)
    {
        HandleCallCount++;
        return Task.FromResult(Result.Success());
    }
}