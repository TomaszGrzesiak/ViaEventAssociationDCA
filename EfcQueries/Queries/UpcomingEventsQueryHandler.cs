using System;
using System.Linq;
using System.Threading.Tasks;
using EfcQueries.Models;
using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.QueryContracts.Contracts;
using ViaEventAssociation.Core.QueryContracts.Queries;

namespace EfcQueries.Queries;

public sealed class UpcomingEventsQueryHandler
    : IQueryHandler<UpcomingEventsQuery.Query, UpcomingEventsQuery.Answer>
{
    private readonly VeaReadModelContext _context;
    private readonly ISystemTime _systemTime;

    public UpcomingEventsQueryHandler(
        VeaReadModelContext context,
        ISystemTime systemTime)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _systemTime = systemTime ?? throw new ArgumentNullException(nameof(systemTime));
    }

    public async Task<UpcomingEventsQuery.Answer> HandleAsync(
        UpcomingEventsQuery.Query query)
    {
        if (query.PageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(query.PageNumber));
        if (query.PageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(query.PageSize));

        // Same format you used in GuestProfileQuery
        var now = _systemTime.Now();
        var nowString = now.ToString("yyyy-MM-dd HH:mm");

        // 1) base: only upcoming events
        var baseQuery = _context.VeaEvents
            .Where(e =>
                e.TimeRangeStartTime != null &&
                e.TimeRangeStartTime!.CompareTo(nowString) >= 0);

        // 2) optional title filter
        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            var searchLower = query.SearchText.ToLower();
            baseQuery = baseQuery.Where(e =>
                e.TitleValue != null &&
                e.TitleValue.ToLower().Contains(searchLower));
        }

        // 3) order: earliest first
        baseQuery = baseQuery.OrderBy(e => e.TimeRangeStartTime);

        // 4) paging info
        var totalEvents = await baseQuery.CountAsync();
        var totalPages = totalEvents == 0
            ? 0
            : (int)Math.Ceiling(totalEvents / (double)query.PageSize);

        var skip = (query.PageNumber - 1) * query.PageSize;

        // 5) load a single page (+ attendees and visibility)
        var pageEvents = await baseQuery
            .Skip(skip)
            .Take(query.PageSize)
            .Select(e => new
            {
                e.Id,
                e.TitleValue,
                e.DescriptionValue,
                e.TimeRangeStartTime,
                e.MaxGuestsNoValue,
                e.Visibility,
                AttendeeCount = e.EventParticipants.Count()
            })
            .ToListAsync();

        // 6) map to DTOs on the client side
        const int maxDescriptionLength = 80;

        var items = pageEvents
            .Select(e =>
            {
                var description = e.DescriptionValue ?? string.Empty;
                var snippet = description.Length <= maxDescriptionLength
                    ? description
                    : description[..maxDescriptionLength] + "...";

                var date = e.TimeRangeStartTime!.Substring(0, 10);   // "yyyy-MM-dd"
                var startTime = e.TimeRangeStartTime!.Substring(11, 5); // "HH:mm"

                // adjust if your enum mapping is different
                var isPublic = e.Visibility == 1; // 1 = Public, 2 = Private in your converter. 1 = true, 2 = false.

                // MaxGuestsNoValue is very likely int? from scaffold
                var maxGuests = e.MaxGuestsNoValue;

                return new UpcomingEventsQuery.EventSummaryDto(
                    EventId: e.Id,
                    Title: e.TitleValue!,
                    DescriptionSnippet: snippet,
                    Date: date,
                    StartTime: startTime,
                    AttendeeCount: e.AttendeeCount,
                    MaxGuests: maxGuests,
                    IsPublic: isPublic
                );
            })
            .ToList();

        return new UpcomingEventsQuery.Answer(
            PageNumber: query.PageNumber,
            PageSize: query.PageSize,
            TotalEvents: totalEvents,
            TotalPages: totalPages,
            Events: items);
    }
}
