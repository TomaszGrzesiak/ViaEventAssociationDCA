using Application.AppEntry;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.Features.Event;

public class UpdateEventTitleHandler(IEventRepository eventRepository, IUnitOfWork uow) : ICommandHandler<UpdateEventTitleCommand>
{
    public async Task<Result> HandleAsync(UpdateEventTitleCommand command)
    {
        // 1) Load
        var ev = await eventRepository.GetAsync(command.EventId);
        if (ev is null)
            return Result.Failure(Error.EventNotFound);

        // 2) Business change in the aggregate
        var change = ev.UpdateTitle(command.NewTitle); // domain method returns Result
        if (change.IsFailure)
            return change; // propagate domain error(s)

        // 3) Commit
        await uow.SaveChangesAsync();
        return Result.Success();
    }
}