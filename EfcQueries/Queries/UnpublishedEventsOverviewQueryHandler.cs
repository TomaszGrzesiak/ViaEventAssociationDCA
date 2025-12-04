using System;
using System.Linq;
using System.Threading.Tasks;
using EfcQueries.Models;
using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.QueryContracts.Contracts;
using ViaEventAssociation.Core.QueryContracts.Queries;

namespace EfcQueries.Queries;

public sealed class UnpublishedEventsOverviewQueryHandler
    : IQueryHandler<UnpublishedEventsOverviewQuery.Query, UnpublishedEventsOverviewQuery.Answer>
{
    private readonly VeaReadModelContext _context;

    public UnpublishedEventsOverviewQueryHandler(VeaReadModelContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<UnpublishedEventsOverviewQuery.Answer> HandleAsync(
        UnpublishedEventsOverviewQuery.Query query)
    {
        if (query is null) throw new ArgumentNullException(nameof(query));
        
        const int StatusDraft     = 1;
        const int StatusCancelled = 3;
        const int StatusReady     = 4;

        // Drafts
        var drafts = await _context.VeaEvents
            .Where(e => e.Status == StatusDraft)
            .OrderBy(e => e.TitleValue)
            .Select(e => new UnpublishedEventsOverviewQuery.EventDto(
                e.Id,
                e.TitleValue))
            .ToListAsync();

        // Ready
        var ready = await _context.VeaEvents
            .Where(e => e.Status == StatusReady)
            .OrderBy(e => e.TitleValue)
            .Select(e => new UnpublishedEventsOverviewQuery.EventDto(
                e.Id,
                e.TitleValue))
            .ToListAsync();

        // Cancelled
        var cancelled = await _context.VeaEvents
            .Where(e => e.Status == StatusCancelled)
            .OrderBy(e => e.TitleValue)
            .Select(e => new UnpublishedEventsOverviewQuery.EventDto(
                e.Id,
                e.TitleValue))
            .ToListAsync();

        return new UnpublishedEventsOverviewQuery.Answer(
            Drafts: drafts,
            Ready: ready,
            Cancelled: cancelled);
    }
}
