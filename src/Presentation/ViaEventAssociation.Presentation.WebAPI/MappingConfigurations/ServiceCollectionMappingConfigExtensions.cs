using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Event;

namespace ViaEventAssociation.Presentation.WebAPI.MappingConfigurations;

public static class ServiceCollectionMappingConfigExtensions
{
    public static IServiceCollection RegisterMappingConfigs(this IServiceCollection services)
    {
        // EventDetails mappings
        services.AddScoped<IMappingConfig<ViewSingleEventRequest, EventDetailsQuery.Query>, ViewSingleEventRequestToQueryMapping>();
        services.AddScoped<IMappingConfig<EventDetailsQuery.Answer, ViewSingleEventResponse>, EventDetailsAnswerToResponseMapping>();

        // Add more mappings as you add more endpoints

        return services;
    }
}