using ViaEventAssociation.Core.QueryContracts.Contracts;

namespace ViaEventAssociation.Core.QueryContracts.Queries;

public static class EventDetailsQuery
{
    // ---- QUERY ----
    public sealed record Query(string EventId, int Offset, int PageSize) : IQuery<Answer>;

    // ---- ANSWER ----
    public sealed record Answer(
        string EventId,
        string Title,
        string Description,
        string LocationName,
        string Date,// "yyyy-MM-dd"
        string StartTime,// "HH:mm"
        string EndTime,// "HH:mm"
        bool IsPublic,
        int AttendeeCount,
        int MaxGuests,
        int TotalGuests, // total guests in list
        int Offset,           // echo back
        int PageSize,         // echo back
        IReadOnlyCollection<GuestDto> Guests);

    public sealed record GuestDto(
        string GuestId,
        string FirstName,
        string LastName,
        string ProfilePictureUrl);
}