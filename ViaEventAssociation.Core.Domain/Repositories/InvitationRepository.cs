using ViaEventAssociation.Core.Domain.Aggregates.Events.Entities;

namespace ViaEventAssociation.Core.Domain.Repositories;

public class InvitationRepository : IInvitationRepository
{
    private readonly List<Invitation> _invitations = new();
    
    public Task AddAsync(Invitation invitation)
    {
        _invitations.Add(invitation);
        return Task.CompletedTask;
    }

    public Task<Invitation> GetAsync(InvitationId id)
    {
        var invitation = _invitations.FirstOrDefault(i => i.Id.Equals(id));
        return Task.FromResult(invitation);
    }
}