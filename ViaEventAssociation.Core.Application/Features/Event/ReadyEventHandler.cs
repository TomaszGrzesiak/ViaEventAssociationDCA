using Application.AppEntry;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.Features.Event;

public class ReadyEventHandler(IEventRepository eventRepository, IUnitOfWork uow, ISystemTime systemTime) : ICommandHandler<ReadyEventCommand>
{
    public async Task<Result> HandleAsync(ReadyEventCommand command)
    {
        var ev = await eventRepository.GetAsync(command.EventId);
        if (ev is null) return Result.Failure(Error.EventNotFound);

        var result = ev.ReadyEvent(systemTime);
        if (result.IsFailure) return result;

        await uow.SaveChangesAsync();
        return result;
    }
}