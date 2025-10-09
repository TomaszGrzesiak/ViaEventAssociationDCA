using Application.AppEntry;
using Microsoft.Extensions.DependencyInjection;
using ViaEventAssociation.Core.Application.AppEntry;
using ViaEventAssociation.Core.Application.AppEntry.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;
using ViaEventAssociation.Core.Tools.OperationResult;

namespace UnitTests.Common.Dispatcher;

public class DispatcherInteractionTest
{
    // zero handlers registered
    [Fact]
    public async Task Dispatch_WithNoRegisteredHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection();
        var sp = services.BuildServiceProvider(); // service provider built with 0 handlers - without registering any handler before building
        var sut = new CommandDispatcher(sp);
        var cmd = CreateEventCommand.Create(Guid.NewGuid().ToString()).Payload!;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.DispatchAsync(cmd));
    }
    
    // one incorrect handler registered
    [Fact]
    public async Task Dispatch_WithRegisteredIncorrectHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        IServiceCollection serviceCollection = new ServiceCollection();
        var incorrectMockHandler = new IncorrectMockHandler();
        serviceCollection.AddScoped<ICommandHandler<None>>(spy => incorrectMockHandler);
        
        //serviceCollection.AddScoped<ICommandHandler<CreateEventCommand>, CreateEventMockHandler>();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        //var handlerSpy = (CreateEventMockHandler) serviceProvider.GetRequiredService<ICommandHandler<CreateEventCommand>>();
        
        var sut = new CommandDispatcher(serviceProvider);
        var cmd = CreateEventCommand.Create(Guid.NewGuid().ToString()).Payload!;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.DispatchAsync(cmd));
    }
    
    // one correct handler registered + Was the handler called exactly once?
    [Fact]
    public async Task Dispatch_ForRegisteredCorrectHandler_CallsExactlyOnceWithSameInstance()
    {
        // Arrange
        IServiceCollection serviceCollection = new ServiceCollection();
        var handlerSpy = new CreateEventMockHandler();
        serviceCollection.AddScoped<ICommandHandler<CreateEventCommand>>(spy => handlerSpy);
        //serviceCollection.AddScoped<ICommandHandler<CreateEventCommand>, CreateEventMockHandler>();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        //var handlerSpy = (CreateEventMockHandler) serviceProvider.GetRequiredService<ICommandHandler<CreateEventCommand>>();
        
        // sut = System Under Test, or in other words: The class I’m currently testing
        var sut = new CommandDispatcher(serviceProvider); // uses your existing dispatcher

        var guid =  Guid.NewGuid();
        var cmd = CreateEventCommand.Create(guid.ToString()).Payload!;

        // Act
        var result = await sut.DispatchAsync(cmd);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, handlerSpy.HandleCallCount); // Was the handler called exactly once
        Assert.Same(cmd, handlerSpy.LastCommand);
    }
    
    // multiple handlers, including correct, registered + Did the dispatcher call only the correct handler
    [Fact]
    public async Task Dispatch_ForRegisteredMultipleHandlersAndCorrectOne_CallsExactlyOnceWithSameInstance()
    {
        // Arrange
        IServiceCollection serviceCollection = new ServiceCollection();
        var handlerSpy = new CreateEventMockHandler();
        var incorrectMockHandler = new IncorrectMockHandler();
        serviceCollection.AddScoped<ICommandHandler<CreateEventCommand>>(spy => handlerSpy);
        //serviceCollection.AddScoped<ICommandHandler<CreateEventCommand>, CreateEventMockHandler>();
        serviceCollection.AddScoped<ICommandHandler<None>>(spy => incorrectMockHandler);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        //var handlerSpy = (CreateEventMockHandler) serviceProvider.GetRequiredService<ICommandHandler<CreateEventCommand>>();
        
        // sut = System Under Test, or in other words: The class I’m currently testing
        var sut = new CommandDispatcher(serviceProvider); // uses your existing dispatcher

        var guid =  Guid.NewGuid();
        var cmd = CreateEventCommand.Create(guid.ToString()).Payload!;

        // Act
        var result = await sut.DispatchAsync(cmd);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, handlerSpy.HandleCallCount);
        Assert.Same(cmd, handlerSpy.LastCommand);
        
        Assert.Equal(0, incorrectMockHandler.HandleCallCount);// Did the dispatcher call only the correct handler?
    }
    
    // multiple handlers, excluding correct, registered
    [Fact]
    public async Task Dispatch_ForRegisteredMultipleHandlersAllIncorrect_CallsExactlyOnceWithSameInstance()
    {
        // Arrange
        IServiceCollection serviceCollection = new ServiceCollection();
        
        var incorrectMockHandler = new IncorrectMockHandler();
        var incorrectMockHandler2 = new IncorrectMockHandler2();
        
        serviceCollection.AddScoped<ICommandHandler<None>>(_ => incorrectMockHandler);
        serviceCollection.AddScoped<ICommandHandler<None>>(_ => incorrectMockHandler2);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        var sut = new CommandDispatcher(serviceProvider);

        var guid =  Guid.NewGuid();
        var cmd = CreateEventCommand.Create(guid.ToString()).Payload!;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.DispatchAsync(cmd));
    }
}
