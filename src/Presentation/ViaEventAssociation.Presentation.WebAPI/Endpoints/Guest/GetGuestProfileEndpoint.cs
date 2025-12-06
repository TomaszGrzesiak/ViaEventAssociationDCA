using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.QueryContracts.QueryDispatching;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebAPI.Endpoints.Guest;

public sealed record GuestProfileRequest(
    [FromRoute] string Id);

public sealed record GuestProfileResponse(
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

public class GetGuestProfileEndpoint(IQueryDispatcher dispatcher, IObjectMapper mapper)
    : ApiEndpoint
        .WithRequest<GuestProfileRequest>
        .AndResults<Ok<GuestProfileResponse>, NotFound>
{
    [HttpGet("guests/{Id}")]
    public override async Task<Results<Ok<GuestProfileResponse>, NotFound>> HandleAsync(
        [FromRoute] GuestProfileRequest request)
    {
        var query = mapper.Map<GuestProfileQuery.Query>(request);

        GuestProfileQuery.Answer answer;
        try
        {
            answer = await dispatcher.DispatchAsync(query); // this de facto Handlers the query with a registered in DI handler (see Program.cs)
        }
        catch (InvalidOperationException ex)
            when (ex.Message.StartsWith("Guest with id", StringComparison.Ordinal))
        {
            return TypedResults.NotFound();
        }

        var response = mapper.Map<GuestProfileResponse>(answer);
        return TypedResults.Ok(response);
    }
    
}