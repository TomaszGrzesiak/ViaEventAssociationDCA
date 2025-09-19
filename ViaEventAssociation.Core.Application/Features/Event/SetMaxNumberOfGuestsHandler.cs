using Application.AppEntry;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.Features.Event;

public class SetMaxNumberOfGuestsHandler(IEventRepository eventRepository, IUnitOfWork uow) : ICommandHandler<SetMaxNumberOfGuestsCommand>
{
    public async Task<Result> HandleAsync(SetMaxNumberOfGuestsCommand command)
    {
        //Load
        var ev = await eventRepository.GetAsync(command.EventId);
        if (ev == null) return Result.Failure(Error.EventNotFound);

        //Business change
        var result = ev.UpdateMaxGuests(command.MaxGuests);
        if (result.IsFailure) return result;

        //Save the changes
        await uow.SaveChangesAsync();
        return Result.Success();
    }
}