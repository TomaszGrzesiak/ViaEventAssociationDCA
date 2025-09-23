using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Contracts;

namespace UnitTests.Fakes;

public class FakeUniqueEmailChecker(List<Guest> guests) : IEmailUnusedChecker
{
    public Task<bool> IsUniqueAsync(EmailAddress emailAddress)
    {
        var isUnique = !guests.Any(g => g.Email.Equals(emailAddress));
        return Task.FromResult(isUnique);
    }
}