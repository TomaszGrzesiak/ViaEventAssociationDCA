using Application.AppEntry;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.Features.Event;

public class ActivateEventHandler(IEventRepository repository, IUnitOfWork unitOfWork, ISystemTime systemTime) : ICommandHandler<ActivateEventCommand>
{
    public async Task<Result> HandleAsync(ActivateEventCommand command)
    {
        var veaEvent = await repository.GetAsync(command.EventId);

        if (veaEvent == null)
            return Result.Failure(Error.EventNotFound);

        var result = veaEvent.Activate(systemTime);

        if (!result.IsSuccess)
            return Result.Failure(result.Errors);

        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}