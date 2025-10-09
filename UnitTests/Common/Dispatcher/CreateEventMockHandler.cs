using Application.AppEntry;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Common.Dispatcher;

public class CreateEventMockHandler: ICommandHandler<CreateEventCommand>
{
    /// <summary>
    /// Spy/Mock handler for interaction tests.
    /// Records call count and the last received command.
    /// </summary>

    public int HandleCallCount { get; private set; }

    public CreateEventCommand? LastCommand { get; private set; }

    public Task<Result> HandleAsync(CreateEventCommand command)
    {
        HandleCallCount++;
        LastCommand = command;
        return Task.FromResult(Result.Success());
    }
}