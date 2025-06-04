using ViaEventAssociation.Core.Domain.Aggregates.Events.Entities;

namespace ViaEventAssociation.Core.Domain.Repositories;

public interface IInvitationRepository
{
    public Task AddAsync(Invitation invitation);
    public Task<Invitation> GetAsync(InvitationId id);
}