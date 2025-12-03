using EfcQueries.Models;
using EfcQueries.SeedFactories;

namespace EfcQueries;

public static class ReadModelDatabaseSetup
{
    public static VeaReadModelContext Seed(this VeaReadModelContext context)
    {
        // Guests
        context.Guests.AddRange(GuestSeedFactory.CreateGuests()); 

        // Events
        var veaEvents = EventSeedFactory.CreateEvents();
        context.VeaEvents.AddRange(veaEvents);
        context.SaveChanges();

        // Participations (many-to-many via nav props)
        ParticipationSeedFactory.Seed(context);
        context.SaveChanges();

        // Invitations
        InvitationSeedFactory.Seed(context);
        context.SaveChanges();

        return context;
    }
}