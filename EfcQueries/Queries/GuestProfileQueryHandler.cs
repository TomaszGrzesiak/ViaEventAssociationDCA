using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EfcQueries.Models;
using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.QueryContracts.Contracts;
using ViaEventAssociation.Core.QueryContracts.Queries;

namespace EfcQueries.Queries;

public sealed class GuestProfileQueryHandler
    : IQueryHandler<GuestProfileQuery.Query, GuestProfileQuery.Answer>
{
    private readonly VeaReadModelContext _context;
    private readonly ISystemTime _systemTime;

    public GuestProfileQueryHandler(VeaReadModelContext context, ISystemTime systemTime)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _systemTime = systemTime ?? throw new ArgumentNullException(nameof(systemTime));
    }

    public async Task<GuestProfileQuery.Answer> HandleAsync(
        GuestProfileQuery.Query query)
    {
        // 1) Load guest basic data
        var guest = await _context.Guests
            .SingleOrDefaultAsync(g => g.Id == query.GuestId);
        
        if (guest is null)
        {
            throw new InvalidOperationException(
                $"Guest with id '{query.GuestId}' was not found in the read model.");
        }

        // 2) "Now" as string matching DB format: "yyyy-MM-dd HH:mm"
        var now = _systemTime.Now();
        var nowString = now.ToString("yyyy-MM-dd HH:mm");

        // 3) Upcoming events this guest participates in
        var upcomingEventsQuery = _context.VeaEvents
            .Where(e =>
                e.TimeRangeStartTime != null &&
                e.TimeRangeStartTime!.CompareTo(nowString) >= 0 &&
                e.EventParticipants.Any(ep => ep.GuestId == query.GuestId))
            .OrderBy(e => e.TimeRangeStartTime)
            .Select(e => new
            {
                e.Id,
                e.TitleValue,
                e.TimeRangeStartTime,
                AttendeeCount = e.EventParticipants.Count()
            });


        var upcomingEvents = await upcomingEventsQuery.ToListAsync();

        // 4) Past events (limit 5, newest first) this guest participated in
        var pastEventsQuery = _context.VeaEvents
            .Where(e =>
                e.TimeRangeStartTime != null &&
                e.TimeRangeStartTime!.CompareTo(nowString) < 0 &&
                e.EventParticipants.Any(ep => ep.GuestId == query.GuestId))
            .OrderByDescending(e => e.TimeRangeStartTime)
            .Take(5)
            .Select(e => new
            {
                e.Id,
                e.TitleValue,
                e.TimeRangeStartTime
            });


        var pastEvents = await pastEventsQuery.ToListAsync();

        // 5) Pending invitations for this guest (enum status = int 1)
        const int PendingStatus = 1;
        var pendingInvitationsCount = await _context.Invitations
            .CountAsync(i => i.GuestId == query.GuestId && i.Status == PendingStatus);

        // 6) Map to DTOs

        var upcomingDtos = upcomingEvents
            .Select(e => new GuestProfileQuery.UpcomingEventDto(
                EventId: e.Id,
                Title: e.TitleValue,
                AttendeeCount: e.AttendeeCount,
                Date: e.TimeRangeStartTime!.Substring(0, 10),   // "yyyy-MM-dd"
                StartTime: e.TimeRangeStartTime!.Substring(11, 5) // "HH:mm"
            ))
            .ToList();

        var pastDtos = pastEvents
            .Select(e => new GuestProfileQuery.PastEventDto(
                EventId: e.Id,
                Title: e.TitleValue
            ))
            .ToList();

        return new GuestProfileQuery.Answer(
            GuestId: guest.Id,
            FirstName: guest.FirstName,
            LastName: guest.LastName,
            Email: guest.Email,
            ProfilePictureUrl: guest.ProfilePictureUrlAddress,
            UpcomingEventsCount: upcomingDtos.Count,
            PendingInvitationsCount: pendingInvitationsCount,
            UpcomingEvents: upcomingDtos,
            PastEvents: pastDtos);
    }
}
