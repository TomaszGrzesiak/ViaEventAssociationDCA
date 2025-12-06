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
        .AndResults<Ok<ViewSingleEventResponse>, NotFound>
{
    [HttpGet("events/{Id}")]
    public override async Task<Results<Ok<ViewSingleEventResponse>, NotFound>> HandleAsync(
        [FromRoute] ViewSingleEventRequest request)
    {
        var query = mapper.Map<EventDetailsQuery.Query>(request);

        EventDetailsQuery.Answer answer;
        try
        {
            answer = await dispatcher.DispatchAsync(query);    
        }     
        catch (InvalidOperationException ex)
            when (ex.Message.StartsWith("Event with id", StringComparison.Ordinal))
        {
            // This is the "not found in read model" case from EventDetailsQueryHandler
            return TypedResults.NotFound();
        }
        
        var response = mapper.Map<ViewSingleEventResponse>(answer);

        return TypedResults.Ok(response);
    }
}