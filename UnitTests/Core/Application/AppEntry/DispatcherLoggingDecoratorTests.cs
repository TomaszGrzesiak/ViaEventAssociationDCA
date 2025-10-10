using Application.AppEntry;
using Microsoft.Extensions.DependencyInjection;
using UnitTests.Common.Dispatcher;
using ViaEventAssociation.Core.Application.AppEntry;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Application.Logger;
using ViaEventAssociation.Core.Domain.Aggregates.Events;

namespace UnitTests.Core.Application.AppEntry;

// --- Test utilities ---
public sealed class FakeLogger : ILogger
{
    public List<string> Infos { get; } = new();
    public List<(string message, Exception exception)> Errors { get; } = new();

    public void Info(string message) => Infos.Add(message);

    public void Error(string message, Exception ex) => Errors.Add((message, ex));
}

public class DispatcherLoggingDecoratorTests
{
     [Fact]
        public async Task Logs_BeforeAndAfter_SuccessfulDispatch()
        {
            // Arrange inner dispatcher + handler
            var services = new ServiceCollection();
            services.AddScoped<ICommandHandler<CreateEventCommand>>(_ => new CreateEventMockHandler());
            var inner = new CommandDispatcher(services.BuildServiceProvider());

            // Arrange logger and decorator
            var logger = new FakeLogger();
            var sut = new DispatcherLoggingDecorator(inner, logger);

            // Arrange command
            var id = Guid.NewGuid();
            CreateEventCommand cmd = CreateEventCommand.Create(id.ToString()).Payload!;

            // Act
            var result = await sut.DispatchAsync(cmd);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Collection(logger.Infos,
                s => Assert.StartsWith("Dispatching CreateEventCommand", s),
                s => Assert.StartsWith("Dispatched CreateEventCommand: Success", s)
            );
            Assert.Empty(logger.Errors);
        }

        [Fact]
        public async Task Logs_Error_WhenInnerThrows()
        {
            // Arrange inner dispatcher that throws by having no handler
            var inner = new CommandDispatcher(new ServiceCollection().BuildServiceProvider());
            var logger = new FakeLogger();
            var sut = new DispatcherLoggingDecorator(inner, logger);

            // Arrange command
            var id = Guid.NewGuid();
            CreateEventCommand cmd = CreateEventCommand.Create(id.ToString()).Payload!;
            
            // Act
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await sut.DispatchAsync(cmd));

            // Assert
            Assert.Contains(logger.Infos, s => s.StartsWith("Dispatching CreateEventCommand"));
            Assert.Single(logger.Errors);
            Assert.StartsWith("Dispatched CreateEventCommand: Exception", logger.Errors[0].message);
            Assert.IsType<InvalidOperationException>(logger.Errors[0].exception);
        }
}


// Below the same test, but using an actual file logger

// public class DispatcherLoggingDecoratorTests
// {
    //     private static string NewLogPath()
    //     {
    //         // Unique file per test run
    //         var folder = Path.Combine(AppContext.BaseDirectory, "test-logs");
    //         Directory.CreateDirectory(folder);
    //         return Path.Combine(folder, $"log-{Guid.NewGuid():N}.txt");
    //     }
    //
    //     [Fact]
    //     public async Task Logs_BeforeAndAfter_SuccessfulDispatch_UsingFileLogger()
    //     {
    //         // Arrange inner dispatcher + handler
    //         var services = new ServiceCollection();
    //         services.AddScoped<ICommandHandler<CreateEventCommand>>(_ => new CreateEventMockHandler());
    //         var inner = new CommandDispatcher(services.BuildServiceProvider());
    //
    //         // File logger + decorator
    //         var logPath = NewLogPath();
    //         var logger = new FileLogger(logPath);
    //         
    //         var sut = new DispatcherLoggingDecorator(inner, logger);
    //
    //         // Arrange command
    //         var id = Guid.NewGuid();
    //         var cmd = CreateEventCommand.Create(id.ToString()).Payload!;
    //
    //         // Act
    //         var result = await sut.DispatchAsync(cmd);
    //
    //         // Assert
    //         Assert.True(result.IsSuccess);
    //
    //         var text = await File.ReadAllTextAsync(logPath);
    //         Assert.Contains("Dispatching CreateEventCommand", text);
    //         Assert.Contains("Dispatched CreateEventCommand: Success", text);
    //     }
    //
    //     [Fact]
    //     public async Task Logs_Error_WhenInnerThrows_UsingFileLogger()
    //     {
    //         // Arrange inner dispatcher that throws by having no handler
    //         var inner = new CommandDispatcher(new ServiceCollection().BuildServiceProvider());
    //         var logPath = NewLogPath();
    //         var logger = new FileLogger(logPath);
    //         var sut = new DispatcherLoggingDecorator(inner, logger);
    //
    //         // Arrange command
    //         var id = Guid.NewGuid();
    //         var cmd = CreateEventCommand.Create(id.ToString()).Payload!;
    //
    //         // Act
    //         await Assert.ThrowsAsync<InvalidOperationException>(async () =>
    //             await sut.DispatchAsync(cmd));
    //
    //         // Assert
    //         var text = await File.ReadAllTextAsync(logPath);
    //         Assert.Contains("Dispatching CreateEventCommand", text);
    //         Assert.Contains("Dispatched CreateEventCommand: Exception", text);
    //         // Optional: check that the exception type text is present
    //         Assert.Contains("InvalidOperationException", text);
    //     }
    // }
