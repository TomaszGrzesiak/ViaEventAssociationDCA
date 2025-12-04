using ViaEventAssociation.Core.QueryContracts.Contracts;

namespace ViaEventAssociation.Core.QueryContracts.Queries;

public static class UpcomingEventsQuery
{
    // Request
    public sealed record Query(
        int PageNumber,
        int PageSize,
        string? SearchText
    ): IQuery<Answer>;

    // Response
    public sealed record Answer(
        int PageNumber,
        int PageSize,
        int TotalEvents,
        int TotalPages,
        IReadOnlyList<EventSummaryDto> Events
    );

    // Single event row in the list
    public sealed record EventSummaryDto(
        string EventId,
        string Title,
        string DescriptionSnippet,
        string Date,       // "yyyy-MM-dd"
        string StartTime,  // "HH:mm"
        int AttendeeCount,
        int MaxGuests,
        bool IsPublic      // true = public, false = private
    );
}