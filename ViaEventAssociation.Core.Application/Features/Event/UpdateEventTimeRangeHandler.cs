using Application.AppEntry;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.Features;

public class UpdateEventTimeRangeHandler : ICommandHandler<UpdateEventTimeRangeCommand>
{
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemTime _systemTime;

    public UpdateEventTimeRangeHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork, ISystemTime systemTime)
    {
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
        _systemTime = systemTime;
    }

    public async Task<Result> HandleAsync(UpdateEventTimeRangeCommand command)
    {
        // Load
        var veaEvent = await _eventRepository.GetAsync(command.EventId);
        if (veaEvent == null) return Result.Failure(Error.EventNotFound);

        // Business change
        var result = veaEvent.UpdateTimeRange(command.EventTimeRange, _systemTime);
        if (result.IsFailure) return result;

        // Save changes
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}