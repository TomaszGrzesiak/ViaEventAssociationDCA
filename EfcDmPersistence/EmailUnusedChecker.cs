using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Contracts;

namespace EfcDmPersistence;

public class EmailUnusedChecker(DmContext dmContext) : IEmailUnusedChecker
{
    public async Task<bool> IsUniqueAsync(EmailAddress emailAddress)
    {
        // Example – adjust to your real domain model
        return !await dmContext.Guests.AnyAsync(g => g.Email == emailAddress);
    }
}