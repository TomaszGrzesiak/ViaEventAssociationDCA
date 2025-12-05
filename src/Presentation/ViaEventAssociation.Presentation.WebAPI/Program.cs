using EfcDmPersistence;
using EfcDmPersistence.GuestPersistence;
using EfcDmPersistence.UnitOfWork;
using EfcDmPersistence.VeaEventPersistence;
using EfcQueries.Models;
using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.Application.AppEntry;
using ViaEventAssociation.Core.Application.Logger;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Domain.Aggregates.Guests;
using ViaEventAssociation.Core.Domain.Common;
using ViaEventAssociation.Core.Domain.Contracts;
using ViaEventAssociation.Core.QueryContracts.Contracts;
using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.QueryContracts.QueryDispatching;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebAPI;
using ViaEventAssociation.Presentation.WebAPI.MappingConfigurations;
using ILogger = Microsoft.Extensions.Logging.ILogger;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// services.AddOpenApi();

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

// services.RegisterDispatcher(); ?????

services.AddApplicationServices(); // commands and handlers in AppEntry

services.AddScoped<IQueryDispatcher, QueryDispatcher>();
services.AddScoped<IQueryHandler<EventDetailsQuery.Query, EventDetailsQuery.Answer>, EfcQueries.Queries.EventDetailsQueryHandler>();
services.AddScoped<IQueryHandler<UpcomingEventsQuery.Query, UpcomingEventsQuery.Answer>, EfcQueries.Queries.UpcomingEventsQueryHandler>();
services.AddScoped<IQueryHandler<UnpublishedEventsOverviewQuery.Query, UnpublishedEventsOverviewQuery.Answer>, EfcQueries.Queries.UnpublishedEventsOverviewQueryHandler>();
services.AddScoped<IQueryHandler<GuestProfileQuery.Query, GuestProfileQuery.Answer>, EfcQueries.Queries.GuestProfileQueryHandler>();

// DbContexts
//string connectionString = @"Data Source = D:\Git\VIA\ViaEventAssociationDCA\EfcDmPersistence\VEADatabaseProduction.db";
var connStringWrite = builder.Configuration.GetConnectionString("WriteDatabase") ?? "Data Source=vea_write.db";
var connStringRead = builder.Configuration.GetConnectionString("ReadDatabase") ?? connStringWrite;

services.AddDbContext<DmContext>(options => options.UseSqlite(connStringWrite));
services.AddDbContext<VeaReadModelContext>(options => options.UseSqlite(connStringRead));
services.AddScoped<IUnitOfWork, SqliteUnitOfWork>();

// Object mapper
services.AddObjectMapper();
services.RegisterMappingConfigs();
// Mapping configs will be added in this project later

services.AddScoped<IEventRepository, VeaEventSqliteRepository>();
services.AddScoped<IGuestRepository, GuestSqliteRepository>();
services.AddScoped<ISystemTime, SystemTime>();
services.AddScoped<IEmailUnusedChecker, EmailUnusedChecker>();
services.AddSingleton<ViaEventAssociation.Core.Application.Logger.ILogger>(sp => new FileLogger(Path.Combine(AppContext.BaseDirectory, "logs", "app.log")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.UseAuthorization(); // ?? Chatty didn't suggest that

app.MapControllers();

app.Run();

// Needed for WebApplicationFactory<Program> in tests
public partial class Program { }


// Below services registration by Troels example:
// services.RegisterCommandDispatching();
// services.RegisterCommandHandlers();
// services.RegisterQueryDispatching();
// services.RegisterReadPersistence(connectionString);
// services.RegisterWritePersistence(connectionString);
// services.RegisterMappingConfigs();