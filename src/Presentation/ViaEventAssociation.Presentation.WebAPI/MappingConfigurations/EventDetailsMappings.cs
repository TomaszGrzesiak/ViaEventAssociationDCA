using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Event;

namespace ViaEventAssociation.Presentation.WebAPI.MappingConfigurations;

// Request -> Query
public sealed class ViewSingleEventRequestToQueryMapping
    : IMappingConfig<ViewSingleEventRequest, EventDetailsQuery.Query>
{
    public EventDetailsQuery.Query Map(ViewSingleEventRequest input)
        => new(input.Id, input.Offset, input.PageSize);
}

// Answer -> Response
public sealed class EventDetailsAnswerToResponseMapping
    : IMappingConfig<EventDetailsQuery.Answer, ViewSingleEventResponse>
{
    public ViewSingleEventResponse Map(EventDetailsQuery.Answer input)
    {
        var guests = input.Guests
            .Select(g => new GuestResponseDto(g.GuestId, g.FirstName, g.LastName, g.ProfilePictureUrl))
            .ToList();

        return new ViewSingleEventResponse(
            input.EventId,
            input.Title,
            input.Description,
            input.LocationName,
            input.Date,
            input.StartTime,
            input.EndTime,
            input.IsPublic,
            input.AttendeeCount,
            input.MaxGuests,
            input.TotalGuests,
            input.Offset,
            input.PageSize,
            guests);
    }
}