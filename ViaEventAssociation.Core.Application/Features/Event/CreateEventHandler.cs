using Application.AppEntry;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.Features.Event;

public class CreateEventHandler : ICommandHandler<CreateEventCommand>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _uow;

    public CreateEventHandler(IEventRepository eventRepository, IUnitOfWork uow)
    {
        _eventRepository = eventRepository;
        _uow = uow;
    }

    public async Task<Result> HandleAsync(CreateEventCommand command)
    {
        var result = VeaEvent.Create(command.EventId);
        if (result.IsFailure) return result;

        // if success
        await _eventRepository.AddAsync(result.Payload!);
        await _uow.SaveChangesAsync();

        return result;
    }
}