using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.Domain.Aggregates.Events;

namespace EfcDmPersistence.VeaEventPersistence;

public class VeaEventSqliteRepository(DbContext context) :
    RepositoryEfcBase<VeaEvent, EventId>(context),
    IEventRepository
{
    private readonly DbContext _context = context;

    public override Task<VeaEvent?> GetAsync(EventId eventId)
    {
        //Load aggregate with owned collections
        return _context.Set<VeaEvent>()
            .Include(e => e.Invitations)
            .Include(e => e.GuestList)
            .SingleOrDefaultAsync(e => e.Id == eventId);
    }
} 