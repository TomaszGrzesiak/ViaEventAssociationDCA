using ViaEventAssociation.Core.QueryContracts.Contracts;

namespace ViaEventAssociation.Core.QueryContracts.Queries;

/// <summary>
/// Query + result types for the personal profile page
/// </summary>
public static class GuestProfileQuery
{
    // ---- QUERY ----
    public sealed record Query(string GuestId) : IQuery<Answer>;

    // ---- ANSWER ----
    public sealed record Answer(
        string GuestId,
        string FirstName,
        string LastName,
        string Email,
        string ProfilePictureUrl,
        int UpcomingEventsCount,
        int PendingInvitationsCount,
        IReadOnlyList<UpcomingEventDto> UpcomingEvents,
        IReadOnlyList<PastEventDto> PastEvents);

    public sealed record UpcomingEventDto(
        string EventId,
        string Title,
        int AttendeeCount,
        string Date,      // "yyyy-MM-dd"
        string StartTime  // "HH:mm"
    );

    public sealed record PastEventDto(
        string EventId,
        string Title
    );
}