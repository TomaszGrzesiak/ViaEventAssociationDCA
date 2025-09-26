using Application.AppEntry;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Guest;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.Features.Guest;

public class RegisterGuestHandler(IGuestRepository guestRepository, IUnitOfWork uow, IEmailUnusedChecker checker) : ICommandHandler<RegisterGuestCommand>
{
    public async Task<Result> HandleAsync(RegisterGuestCommand cmd)
    {
        var id = GuestId.CreateUnique();
        // 1) Load - no load when registering

        // 2) Business change in the aggregate
        var result = await Domain.Aggregates.Guests.Guest.Register(cmd.GuestId, cmd.EmailAddress, cmd.FirstName, cmd.LastName, cmd.ProfilePictureUrlAddress,
            checker);
        if (result.IsFailure) return result;

        // 3) Commit
        await guestRepository.AddAsync(result.Payload!);
        await uow.SaveChangesAsync();
        return Result.Success();
    }
}