using Application.AppEntry;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.Features.Event;

public class UpdateEventTitleHandler : ICommandHandler<UpdateEventTitleCommand>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _uow;

    public UpdateEventTitleHandler(IEventRepository eventRepository, IUnitOfWork uow)
    {
        _eventRepository = eventRepository;
        _uow = uow;
    }

    public async Task<Result> HandleAsync(UpdateEventTitleCommand command)
    {
        // 1) Load
        var ev = await _eventRepository.GetAsync(command.EventId);
        if (ev is null)
            return Result.Failure(Error.CantFindEventWithThisId);

        // 2) Business change in the aggregate
        var change = ev.UpdateTitle(command.NewTitle); // domain method returns Result
        if (change.IsFailure)
            return change; // propagate domain error(s)

        // 3) Commit
        await _uow.SaveChangesAsync();
        return Result.Success();
    }
}