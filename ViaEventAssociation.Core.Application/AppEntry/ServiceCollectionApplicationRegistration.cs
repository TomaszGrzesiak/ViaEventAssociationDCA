using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.Application.Logger;

namespace ViaEventAssociation.Core.Application.AppEntry;

public static class ServiceCollectionApplicationRegistration
{
    /// <summary>
    /// One entry-point from the Web layer. Keeps Program.cs ignorant of inner details.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // 1) Auto-register all ICommandHandler<T> in THIS assembly
        services.AddCommandHandlersFrom(Assembly.GetExecutingAssembly());

        // 2) (Optional) Register the dispatcher + decorators here too, to keep it centralized:
        services.AddScoped<ICommandDispatcher>(sp =>
        {
            var inner = new CommandDispatcher(sp);
            var logger = sp.GetRequiredService<ILogger>();
            return new DispatcherLoggingDecorator(inner, logger);
        });

        // 3) (Optional) Any other Application-level services (validators, mappers, etc.)

        return services;
    }
}

// Program.cs (Web/API project)
//
// Now the Web layer is clean and decoupled:
//
// using ViaEventAssociation.Core.Application.AppEntry;
//
// var builder = WebApplication.CreateBuilder(args);
// var services = builder.Services;
//
// services.AddApplicationServices();   // <- single, high-level call
//
// var app = builder.Build();
// app.Run();