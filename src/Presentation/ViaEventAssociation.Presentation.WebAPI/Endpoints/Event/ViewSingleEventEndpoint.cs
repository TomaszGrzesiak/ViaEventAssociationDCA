using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.QueryContracts.QueryDispatching;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebAPI.Endpoints.Event;

// Request DTO (what Web API binds)
public sealed record ViewSingleEventRequest(
    [FromRoute] string Id,
    [FromQuery] int Offset = 0,
    [FromQuery] int PageSize = 10);

// Response DTO (what Web API returns)
public sealed record ViewSingleEventResponse(
    string EventId,
    string Title,
    string Description,
    string LocationName,
    string Date,
    string StartTime,
    string EndTime,
    bool IsPublic,
    int AttendeeCount,
    int MaxGuests,
    int TotalGuests,
    int Offset,
    int PageSize,
    IReadOnlyCollection<GuestResponseDto> Guests);

public sealed record GuestResponseDto(
    string GuestId,
    string FirstName,
    string LastName,
    string ProfilePictureUrl);

public sealed class ViewSingleEventEndpoint(
    IQueryDispatcher dispatcher,
    IObjectMapper mapper)
    : ApiEndpoint
        .WithRequest<ViewSingleEventRequest>
        .AndResult<Ok<ViewSingleEventResponse>>
{
    [HttpGet("events/{Id}")]
    public override async Task<Ok<ViewSingleEventResponse>> HandleAsync(
        [FromRoute] ViewSingleEventRequest request)
    {
        // Map request -> query
        var query = mapper.Map<EventDetailsQuery.Query>(request);

        var answer = await dispatcher.DispatchAsync(query);

        // Map answer -> response
        var response = mapper.Map<ViewSingleEventResponse>(answer);

        return TypedResults.Ok(response);
    }
}