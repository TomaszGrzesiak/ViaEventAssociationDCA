using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;

namespace EfcDmPersistence.GuestPersistence;

public class GuestSqliteRepository(DbContext context) :
    RepositoryEfcBase<Guest, GuestId>(context),
    IGuestRepository
{
}