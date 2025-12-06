using System.Net;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Guest;
using Xunit;

namespace IntegrationTests.WebAPI.Guest;

public class GuestProfileEndpointTests
{
    [Fact]
    public async Task GetGuestProfile_ExistingGuest_ShouldReturnOk()
    {
        await using var factory = new VeaWebApplicationFactory();
        var client = factory.CreateClient();
        
        // "Id": "6f6e4b5a-0114-4be6-892c-a8fe015d702a",
        // "FirstName": "Abdel",
        // "LastName": "Abbott",
        // "Email": "325031@via.dk",
        
        var guestId = "6f6e4b5a-0114-4be6-892c-a8fe015d702a";
        
        // ACT
        var response = await client.GetAsync($"/api/guests/{guestId}");

        // ASSERT HTTP
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // ASSERT BODY
        var body = await response.Content.ReadFromJsonAsync<GuestProfileResponse>();
        Assert.NotNull(body);
        Assert.Equal(guestId, body!.GuestId);
        Assert.Equal("Abbott", body.LastName);
    }
    
    [Fact]
    public async Task GetGuestProfile_NonExistingGuest_ShouldReturnNotFound()
    {
        await using var factory = new VeaWebApplicationFactory();
        var client = factory.CreateClient();

        var missingId = Guid.NewGuid().ToString();

        // ACT
        var response = await client.GetAsync($"/api/guests/{missingId}");

        // ASSERT
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}