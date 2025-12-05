using Microsoft.Extensions.DependencyInjection;

namespace ViaEventAssociation.Core.Tools.ObjectMapper;

public static class ServiceCollectionObjectMapperExtensions
{
    public static IServiceCollection AddObjectMapper(this IServiceCollection services)
    {
        services.AddScoped<IObjectMapper, ObjectMapper>();
        return services;
    }
}