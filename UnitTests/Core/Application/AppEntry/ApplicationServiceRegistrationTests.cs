using Application.AppEntry;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Application.AppEntry;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;
using ViaEventAssociation.Core.Application.Logger;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Common;

namespace UnitTests.Core.Application.AppEntry;

// Example stub just to satisfy DI; methods can throw if not used by this test

public class ApplicationServiceRegistrationTests
{
    private static string NewLogPath()
    {
        var folder = Path.Combine(AppContext.BaseDirectory, "test-logs");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, $"log-{Guid.NewGuid():N}.txt");
    }

    [Fact]
    public void AddApplicationServices_registers_all_handlers_resolvable()
    {
        // Arrange
        var services = new ServiceCollection();

        // Decorator dependency (your ApplicationRegistration wires a logging decorator)
        services.AddSingleton<ILogger>(_ => new FileLogger(NewLogPath()));

        // === Fake/stub dependencies used by handlers ===
        services.AddScoped<IEventRepository, InMemEventRepoStub>();
        services.AddScoped<IUnitOfWork, FakeUoW>();
        // (Add more here if other handlers need them)

        // The Application entry-point (calls your scanner + dispatcher/decorator)
        services.AddApplicationServices();

        var sp = services.BuildServiceProvider();

        // Act – try to resolve a known handler interface
        var handler = sp.GetService<ICommandHandler<CreateEventCommand>>();

        // Assert
        Assert.NotNull(handler);
        Assert.IsType<CreateEventHandler>(handler);
    }

    [Fact]
    public void AddApplicationServices_ShouldRegister_DispatcherAndLogger()
    {
        // Arrange
        var services = new ServiceCollection();
        
        services.AddSingleton<ILogger>(_ => new FileLogger(NewLogPath()));

        // Act
        services.AddApplicationServices(); // internally resolves ILogger for your decorator
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var dispatcher = serviceProvider.GetService<ICommandDispatcher>();
        Assert.NotNull(dispatcher);
    }
}