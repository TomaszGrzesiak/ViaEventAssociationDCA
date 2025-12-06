using System.Diagnostics;
using System.Net;
using System.Text.Json;
using EfcDmPersistence;
using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Event;
using Xunit;
using Xunit.Abstractions;
using EventId = ViaEventAssociation.Core.Domain.Aggregates.Events.EventId;

namespace IntegrationTests.WebAPI.Event;

public class UpdateEventTitleEndpointTests(ITestOutputHelper output)
{
    [Fact]
    public async Task UpdateTitle_ValidInput_ShouldReturnNoContent()
    {
        await using var webApp = new VeaWebApplicationFactory();
        var client = webApp.CreateClient();

        // create event first through WebAPI
        var createResponse = await client.PostAsync("/api/events/create", JsonContent.Create(new { }));
        var createDto = await createResponse.Content.ReadFromJsonAsync<CreateEventResponse>();
        var eventId = createDto!.Id;

        var body = new UpdateTitleBody("New Title");

        // act
        var response = await client.PostAsJsonAsync($"/api/events/{eventId}/update-title", body);

        // assert HTTP
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode); // NoContent is also a SUCCESS (code 204)

        // assert DB
        using var scope = webApp.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DmContext>();
        var eventIdResult = EventId.FromString(eventId);
        var veaEvent = await ctx.VeaEvents.SingleAsync(e => e.Id == eventIdResult.Payload);
        Assert.Equal("New Title", veaEvent.Title.Value);
    }
    
    [Fact]
    public async Task UpdateTitle_InvalidInput_ShouldReturnBadRequest()
    {
        await using var webApp = new VeaWebApplicationFactory();
        var client = webApp.CreateClient();

        // create event first through WebAPI
        var createResponse = await client.PostAsync("/api/events/create", JsonContent.Create(new { }));
        var createDto = await createResponse.Content.ReadFromJsonAsync<CreateEventResponse>();
        var eventId = createDto!.Id;

        var bodyWithWrongTitle = new UpdateTitleBody("AC");

        // act
        var response = await client.PostAsJsonAsync($"/api/events/{eventId}/update-title", bodyWithWrongTitle);

        // assert HTTP
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        output.WriteLine(await response.Content.ReadAsStringAsync());
        var json = await response.Content.ReadAsStringAsync();
        var errors = JsonSerializer.Deserialize<List<ErrorDto>>(json)!;
        Assert.NotNull(errors);
        Assert.Single(errors); // optional, but good discipline
        Assert.Equal(102, errors![0].code);
        
        // assert DB (no changes in the Title value)
        using var scope = webApp.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<DmContext>();
        var eventIdResult = EventId.FromString(eventId);
        var veaEvent = await ctx.VeaEvents.SingleAsync(e => e.Id == eventIdResult.Payload);
        Assert.NotEqual(bodyWithWrongTitle.Title, veaEvent.Title.Value);
    }
}
