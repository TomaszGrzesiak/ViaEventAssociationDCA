using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Event;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Guest;

namespace ViaEventAssociation.Presentation.WebAPI.MappingConfigurations;

public static class ServiceCollectionMappingConfigExtensions
{
    public static IServiceCollection RegisterMappingConfigs(this IServiceCollection services)
    {
        // EventDetails mappings
        services.AddScoped<IMappingConfig<ViewSingleEventRequest, EventDetailsQuery.Query>, ViewSingleEventRequestToQueryMapping>();
        services.AddScoped<IMappingConfig<EventDetailsQuery.Answer, ViewSingleEventResponse>, EventDetailsAnswerToResponseMapping>();
        services.AddScoped<IMappingConfig<GuestProfileRequest, GuestProfileQuery.Query>, GuestProfileRequestToQueryMapping>();
        services.AddScoped<IMappingConfig<GuestProfileQuery.Answer, GuestProfileResponse>,  GuestProfileAnswerToResponseMapping>();

        // Add more mappings as you add more endpoints

        return services;
    }
}