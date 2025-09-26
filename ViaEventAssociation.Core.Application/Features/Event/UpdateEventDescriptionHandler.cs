using Application.AppEntry;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.Features.Event;

public class UpdateEventDescriptionHandler : ICommandHandler<UpdateEventDescriptionCommand>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _uow;

    public UpdateEventDescriptionHandler(IEventRepository eventRepository, IUnitOfWork uow)
    {
        _eventRepository = eventRepository;
        _uow = uow;
    }

    public async Task<Result> HandleAsync(UpdateEventDescriptionCommand command)
    {
        // Load
        var ev = await _eventRepository.GetAsync(command.EventId);
        if (ev == null)
            return Result.Failure(Error.EventNotFound);

        // Business change in aggregate
        var result = ev.UpdateDescription(command.EventDescription);
        if (result.IsFailure) return result;

        // Save the changes
        await _uow.SaveChangesAsync();
        return Result.Success();
    }
}