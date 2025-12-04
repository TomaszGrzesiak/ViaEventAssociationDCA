using System;
using System.Linq;
using System.Threading.Tasks;
using EfcQueries.Models;
using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.QueryContracts.Contracts;
using ViaEventAssociation.Core.QueryContracts.Queries;

namespace EfcQueries.Queries;

public sealed class EventDetailsQueryHandler
    : IQueryHandler<EventDetailsQuery.Query, EventDetailsQuery.Answer>
{
    private readonly VeaReadModelContext _context;

    public EventDetailsQueryHandler(VeaReadModelContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<EventDetailsQuery.Answer> HandleAsync(
        EventDetailsQuery.Query query)
    {
        if (query is null) throw new ArgumentNullException(nameof(query));

        // 1) Load basic event data + attendee count
        var evt = await _context.VeaEvents
            .Where(e => e.Id == query.EventId)
            .Select(e => new
            {
                e.Id,
                e.TitleValue,
                e.DescriptionValue,
                e.TimeRangeStartTime,
                e.TimeRangeEndTime,
                e.Visibility,
                e.MaxGuestsNoValue,
                AttendeeCount = 0
            })
            .SingleAsync();

        // 2. Make a list of Guests participating the event (+these who accepted Invitation)
        // A) GuestIds from EventParticipants (already attending)
        var participantGuestIdsQuery =
            _context.EventParticipants
                .Where(ep => ep.EventId == query.EventId)
                .Select(ep => ep.GuestId);

        // B) GuestIds from Invitations with "Accepted" status
        const int InvitationAcceptedStatus = 2; // Enum "Accepted" = 2 in DB

        var acceptedInvitationGuestIdsQuery =
            _context.Invitations
                .Where(i => i.EventId == query.EventId && i.Status == InvitationAcceptedStatus)
                .Select(i => i.GuestId);

        // C) Union + distinct = all *attending* guests for this event
        var attendingGuestIdsQuery =
            participantGuestIdsQuery
                .Union(acceptedInvitationGuestIdsQuery);
        
        // D) Building the actual list of attendees
        var guestsBaseQuery =
            from g in _context.Guests
            where attendingGuestIdsQuery.Contains(g.Id)
            select new
            {
                g.Id,
                g.FirstName,
                g.LastName,
                g.ProfilePictureUrlAddress
            };

        var totalGuests = await guestsBaseQuery.CountAsync();

        var guestsWindow = await guestsBaseQuery
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .Skip(query.Offset)
            .Take(query.PageSize)
            .Select(x => new EventDetailsQuery.GuestDto(
                x.Id,
                x.FirstName,
                x.LastName,
                x.ProfilePictureUrlAddress
            ))
            .ToListAsync();

        // 3) String slicing for view-friendly date/time
        var date = evt.TimeRangeStartTime?.Substring(0, 10) ?? string.Empty;   // "yyyy-MM-dd"
        var startTime = evt.TimeRangeStartTime?.Substring(11, 5) ?? string.Empty; // "HH:mm"
        var endTime = evt.TimeRangeEndTime?.Substring(11, 5) ?? string.Empty;     // "HH:mm"

        // 4) Visibility: assume 1 = Public, 2 = Private (same mapping as your seed)
        bool isPublic = evt.Visibility == 1;
        
        var attendeeCount = await attendingGuestIdsQuery.CountAsync();

        return new EventDetailsQuery.Answer(
            EventId: evt.Id,
            Title: evt.TitleValue,
            Description: evt.DescriptionValue,
            LocationName: "C05.19",
            Date: date,
            StartTime: startTime,
            EndTime: endTime,
            IsPublic: isPublic,
            AttendeeCount:  attendeeCount,
            MaxGuests: evt.MaxGuestsNoValue,
            TotalGuests: totalGuests,
            Offset: query.Offset,
            PageSize: query.PageSize,
            Guests: guestsWindow);
    }
}