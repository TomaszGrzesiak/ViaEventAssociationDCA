using Application.AppEntry;
using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;

namespace ViaEventAssociation.Core.Application.Extensions;

public static class ApplicationExtensions
{
    public static void RegisterHandlers(this IServiceCollection services)
    {
        
        // COMMANDS AND HANDLERS SHOULD BE REGISTERED AUTOMATICALLY FROM ASSEMBLY
        
       // services.AddScoped<ICommandHandler<CreateEventCommand>, CreateEventHandler>();
        //services.AddScoped<ICommandHandler<UpdateEventTitleCommand>, UpdateEventTitleHandler>();
        // ...
    }
}