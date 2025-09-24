using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Contracts;

namespace UnitTests.Fakes;

public class FakeUniqueEmailChecker(IEnumerable<string> takenEmails) : IEmailUnusedChecker
{
    private readonly HashSet<string> _takenEmails = new(takenEmails, StringComparer.OrdinalIgnoreCase);

    public Task<bool> IsUniqueAsync(EmailAddress emailAddress)
    {
        var isUnique = !_takenEmails.Contains(emailAddress.ToString());
        return Task.FromResult(isUnique);
    }
}