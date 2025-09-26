using ViaEventAssociation.Core.Domain.Aggregates.Guests;

namespace ViaEventAssociation.Core.Domain.Contracts;

public interface IEmailUnusedChecker
{
    Task<bool> IsUniqueAsync(EmailAddress emailAddress);
}