namespace ViaEventAssociation.Core.Domain.Contracts;

public interface IEmailUnusedChecker
{
    Task<bool> IsEmailInUse(string email);
}