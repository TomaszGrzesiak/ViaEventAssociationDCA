using Application.AppEntry;
using Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Domain.Repositories;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace Application.Features.Event;

public class ActivateEventHandler : ICommandHandler<ActivateEventCommand>
{
    private readonly IEventRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateEventHandler(IEventRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public Task<Result> Handle(ActivateEventCommand command)
    {
        List<string> errorMessages = new List<string>();

        if (command == null)
        {
            errorMessages.Add("Command is null");
            return Task.FromResult(new Result(errorMessages));
        }

        var eventResult = _repository.GetViaEventByIdAsync(command.EventId);

        var @event = eventResult.Result.Payload;

        if (@event == null)
        {
            errorMessages.Add("Event not found");
            return Task.FromResult(new Result(errorMessages));
        }

        var activationResult = @event.Activate();

        if (!activationResult.IsSuccess)
        {
            errorMessages.AddRange(activationResult.ErrorMessages);
            return Task.FromResult(new Result(errorMessages));
        }

        _repository.UpdateAsync(@event);
        _unitOfWork.SaveChangesAsync();

        return Task.FromResult(new Result(errorMessages));
    }
}