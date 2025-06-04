using Application.AppEntry;
using Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Domain.Repositories;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace Application.Features.Event;

public class CreateEventHandler : ICommandHandler<CreateEventCommand>
{
    private readonly IEventRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateEventHandler(IEventRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CreateEventCommand command)
    {
        EventId eventId = new EventId(Guid.Empty);
        var result = ViaEventAssociation.Core.Domain.Aggregates.Events.Event.CreateEvent();
        // ViaEventAssociation.Core.Domain.Aggregates.Events.Event createdEvent = result.Payload;
        ViaEventAssociation.Core.Domain.Aggregates.Events.Event createdEvent = (
            (Result<ViaEventAssociation.Core.Domain.Aggregates.Events.Event>)result
        ).Payload;

        if (result.IsSuccess)
        {
            await _repository.AddAsync(createdEvent);
            var GetEvent = await _repository.GetViaEventByIdAsync(command.Id);

            await _unitOfWork.SaveChangesAsync();

            return new Result();
        }

        return new Result();

    }
}