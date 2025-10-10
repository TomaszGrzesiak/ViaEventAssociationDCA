using System.Reflection;
using Application.AppEntry;
using Microsoft.Extensions.DependencyInjection;

namespace ViaEventAssociation.Core.Application.AppEntry;

public static class ApplicationServiceRegistration
{
    /// <summary>
    /// Scans the provided assembly for concrete types implementing ICommandHandler<>
    /// and registers them as Scoped (one per request/command scope).
    /// </summary>
    /// 
    public static IServiceCollection AddCommandHandlersFrom(
        this IServiceCollection services, Assembly assembly)
    {
        foreach (var type in assembly.GetTypes())
        {
            // skip abstract/interface/open-generic types because they can't be instantiated
            // in the assembly, besides the handlers, there'll be some Interfaces that otherwise would match the next filter. 
            if (type.IsAbstract || type.IsInterface) continue; // I actually don't have any abstract class like BaseHandler<T> : ICommandHandler<T> { ... }

            // find all ICommandHandler<T> interfaces this type implements
            var implemented = type
                .GetInterfaces()
                
                // i.IsGenericType filters out other interfaces, that the classes could potentially implement in the future
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() ==  typeof(ICommandHandler<>)) 
                .ToArray();

            if (implemented.Length == 0) continue;

            foreach (var handlerInterface in implemented)
            {
                services.AddScoped(handlerInterface, type);
            }
        }
        
        return services;
    }
}