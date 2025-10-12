using ViaEventAssociation.Core.Application.Logger;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.AppEntry;


public class DispatcherLoggingDecorator: ICommandDispatcher
{
    /// <summary>
    /// Simple logging decorator for ICommandDispatcher.
    /// Logs before and after dispatch, including failures.
    /// </summary>
    private readonly ICommandDispatcher _inner;
    private readonly ILogger _logger;

    public DispatcherLoggingDecorator(ICommandDispatcher inner, ILogger logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<Result> DispatchAsync<TCommand>(TCommand command)
    {
        var name = typeof(TCommand).Name;
        _logger.Info($"Dispatching {name}");
        try
        {
            var result = await _inner.DispatchAsync(command);
            if (result.IsSuccess)
                _logger.Info($"Dispatched {name}: Success");
            else
                _logger.Info($"Dispatched {name}: Failure");
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error($"Dispatched {name}: Exception", ex);
            throw;
        }
    }
    
}