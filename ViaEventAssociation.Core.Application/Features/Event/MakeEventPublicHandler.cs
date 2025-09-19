using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.Features.Event;

public class MakeEventPublicHandler(IEventRepository eventRepository, IUnitOfWork unitOfWork)
{
    public async Task<Result> HandleAsync(MakeEventPublicCommand command)
    {
        // Load
        var ev = await eventRepository.GetAsync(command.EventId);
        if (ev == null)
            return Result.Failure(Error.EventNotFound);

        // Business change
        var result = ev.UpdateVisibility(EventVisibility.Public);
        if (result.IsFailure) return result;

        // Save the change
        await unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}