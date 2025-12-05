var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// builder.Services.RegisterDispatcher();

string connectionString = @"Data Source = D:\Git\VIA\ViaEventAssociationDCA\EfcDmPersistence\VEADatabaseProduction.db";

// builder.Services.RegisterCommandDispatching();
// builder.Services.RegisterCommandHandlers();
// builder.Services.RegisterQueryDispatching();
// builder.Services.RegisterReadPersistence(connectionString);
// builder.Services.RegisterWritePersistence(connectionString);
// builder.Services.RegisterMappingConfigs();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();