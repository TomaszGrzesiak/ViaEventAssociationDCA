using Application.AppEntry;
using Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Domain.Repositories;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace Application.Features.Event;

public class CancelParticipationHandler : ICommandHandler<CancelParticipationCommand>
{
    private readonly IEventRepository _repository;
    private readonly IGuestRepository _guestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelParticipationHandler(IEventRepository repository, IGuestRepository guestRepository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _guestRepository = guestRepository;
        _unitOfWork = unitOfWork;
    }
    
    public Task<Result> Handle(CancelParticipationCommand command)
    {
        List<string> errorMessages = new List<string>();
        if (command == null)
        {
            errorMessages.Add("Command is null");
            return Task.FromResult(new Result());
        }

        var eventResult = _repository.GetViaEventByIdAsync(new EventId(new Guid()));

        var @event = eventResult.Result.Payload;

        if (@event == null)
        {
            errorMessages.Add("Event not found");
            return Task.FromResult(new Result());
        }

        var guestResult = _guestRepository.GetGuestByIdAsync(new GuestId(new Guid()));
        var @guest = guestResult.Result.Payload;

        if (@guest == null)
        {
            errorMessages.Add("Guest not found");
            return Task.FromResult(new Result());
        }

        var cancelResult = @event.CancelEvent(Guid.NewGuid());
        if (!cancelResult.IsSuccess)
        {
            return Task.FromResult(new Result());
        }

        _repository.UpdateAsync(@event);
        _unitOfWork.SaveChangesAsync();
        
        return Task.FromResult(new Result());
    }
}