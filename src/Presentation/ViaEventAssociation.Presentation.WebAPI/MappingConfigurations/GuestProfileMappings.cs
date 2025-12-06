using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Guest;

namespace ViaEventAssociation.Presentation.WebAPI.MappingConfigurations;

// Request -> Query
public sealed class GuestProfileRequestToQueryMapping : IMappingConfig<GuestProfileRequest, GuestProfileQuery.Query>
{
    public GuestProfileQuery.Query Map(GuestProfileRequest input)
        => new GuestProfileQuery.Query(input.Id);
}

// Answer -> Response
public sealed class GuestProfileAnswerToResponseMapping : IMappingConfig<GuestProfileQuery.Answer, GuestProfileResponse>
{
    public GuestProfileResponse Map(GuestProfileQuery.Answer input)
    {
        var upcomingEventDtos = input.UpcomingEvents
            .Select(e => new UpcomingEventDto(e.EventId, e.Title, e.AttendeeCount, e.Date, e.StartTime))
            .ToList();
        
        var pastEventDtos = input.PastEvents
            .Select(e => new PastEventDto(e.EventId, e.Title))
            .ToList();

        return new GuestProfileResponse(
            input.GuestId,
            input.FirstName,
            input.LastName,
            input.Email,
            input.ProfilePictureUrl,
            input.UpcomingEventsCount,
            input.PendingInvitationsCount,
            upcomingEventDtos,
            pastEventDtos);
    }
}