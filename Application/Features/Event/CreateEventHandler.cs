using Application.AppEntry;
using Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Domain.Repositories;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace Application.Features.Event;

public class CreateEventHandler : ICommandHandler<CreateEventCommand>
{
    private readonly IEventRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    
    public Task<Result> Handle(CreateEventCommand command)
    {
        throw new NotImplementedException();
    }
}