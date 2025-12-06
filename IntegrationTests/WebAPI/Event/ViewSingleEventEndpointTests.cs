using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xunit;
using IntegrationTests.WebAPI;
using EfcQueries.Models;        // <-- read model namespace
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Event;

namespace IntegrationTests.WebAPI.Event;

public class ViewSingleEventEndpointTests
{
    // ------------------------------------------------------------
    // SUCCESS SCENARIO
    // ------------------------------------------------------------
    [Fact]
    public async Task ViewSingleEvent_ExistingEvent_ShouldReturnOk()
    {
        await using var factory = new VeaWebApplicationFactory();
        var client = factory.CreateClient();

        var eventId = Guid.NewGuid().ToString();

        // Seed the READ MODEL directly
        using (var scope = factory.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<VeaReadModelContext>();
            
            // Insert a simple valid event row
            ctx.VeaEvents.Add(new VeaEvent
            {
                Id = eventId,
                TitleValue = "Integration Test Title",
                DescriptionValue = "Test Description",
                TimeRangeStartTime = "2025-01-01T10:00:00",
                TimeRangeEndTime = "2025-01-01T12:00:00",
                Visibility = 1,           // Public
                MaxGuestsNoValue = 50
            });

            await ctx.SaveChangesAsync();
        }

        // ACT
        var response = await client.GetAsync($"/api/events/{eventId}");

        // ASSERT HTTP
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // ASSERT BODY
        var body = await response.Content.ReadFromJsonAsync<ViewSingleEventResponse>();
        Assert.NotNull(body);
        Assert.Equal(eventId, body!.EventId);
        Assert.Equal("Integration Test Title", body.Title);
    }


    // ------------------------------------------------------------
    // FAILURE SCENARIO
    // ------------------------------------------------------------
    [Fact]
    public async Task ViewSingleEvent_NonExistingEvent_ShouldReturnNotFound()
    {
        await using var factory = new VeaWebApplicationFactory();
        var client = factory.CreateClient();

        var missingId = Guid.NewGuid().ToString();

        // ACT
        var response = await client.GetAsync($"/api/events/{missingId}");

        // ASSERT
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    // Fetching "Friday bar" VeaEvent.
    [Fact]
    public async Task ViewSingleEvent_ExistingEventFromTroelsSeed_ShouldReturnOk()
    {
        await using var factory = new VeaWebApplicationFactory();
        var client = factory.CreateClient();

        const string eventId = "40ed2fd9-2240-4791-895f-b9da1a1f64e4";

        // ACT
        var response = await client.GetAsync($"/api/events/{eventId}");

        // ASSERT HTTP
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // ASSERT BODY
        var body = await response.Content.ReadFromJsonAsync<ViewSingleEventResponse>();
        Assert.NotNull(body);
        Assert.Equal(eventId, body!.EventId);
        Assert.Equal("Friday Bar", body.Title);
    }
}
