using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Contracts;

namespace UnitTests.Fakes;

public class FakeUniqueEmailChecker(List<string> takenEmails) : IEmailUnusedChecker
{
    public Task<bool> IsUniqueAsync(EmailAddress emailAddress)
    {
        var isUnique = !takenEmails.Any(t => t.Equals(emailAddress.ToString()));
        return Task.FromResult(isUnique);
    }
}