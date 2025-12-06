using System.Net;
using System.Net.Http.Json;
using EfcDmPersistence;
using IntegrationTests.WebAPI;
using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Event;
using Xunit;
using EventId = ViaEventAssociation.Core.Domain.Aggregates.Events.EventId;


namespace IntegrationTests.WebAPI.Event;

public class CreateEventEndpointTests
{
    // Only one scenario, because there's literally nothing that can go wrong at any stage.
    
    [Fact]
    public async Task CreateEvent_ShouldReturnOk_AndPersistEvent()
    {
        await using var webApp = new VeaWebApplicationFactory();
        var client = webApp.CreateClient();
        
        // act
        var response = await client.PostAsync("/api/events/create", JsonContent.Create(new { }));
        
        // assert (HTTP)
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var dto = await response.Content.ReadFromJsonAsync<CreateEventResponse>();
        Assert.NotNull(dto);
        Assert.False(string.IsNullOrWhiteSpace(dto!.Id));
        
        // assert (DB)
        using var scope = webApp.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DmContext>();
        var eventIdResult = EventId.FromString(dto.Id);
        Assert.True(eventIdResult.IsSuccess);
        
        var veaEvent = await ctx.VeaEvents.SingleOrDefaultAsync(e => e.Id == eventIdResult.Payload);
        Assert.NotNull(veaEvent);
    }
}