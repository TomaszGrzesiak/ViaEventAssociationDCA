using Application.AppEntry;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Domain.Repositories;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace Application.Features.Event;

public class ActivateEventHandler : ICommandHandler<ActivateEventHandler>
{
    private readonly IEventRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateEventHandler(IEventRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ActivateEventHandler command)
    {
        List<string> errorMessage = new List<string>();

        if (command == null)
        {
            return Result.Failure(Error.CommandCannotBeNull);
        }

        var eventResult = _repository.GetViaEventByIdAsync(new EventId(new Guid()));
        var @event = eventResult.Result.Payload;

        if (@event == null)
        {
            errorMessage.Add("Event not found");
            return await Task.FromResult(new Result());
        }

        var activationResult = @event.ActivateTheEvent(new Guid());

        if (!activationResult.IsSuccess)
        {
            //@TODO implement more 
        }
        throw new NotImplementedException();
    }
}