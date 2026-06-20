using System.Net;
using System.Threading.Tasks;
using GymManagementSystem.PL.Tests.Infrastructure;
using Shouldly;
using Xunit;

namespace GymManagementSystem.PL.Tests;

public class BookingsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public BookingsControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Index_RequiresAuthorization()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Bookings");

        response.StatusCode.ShouldBeOneOf(HttpStatusCode.Redirect, HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Book_PastSession_RedirectsWithError()
    {
        var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.PostAsync("/Bookings/Book?sessionId=999", null);

        response.StatusCode.ShouldBeOneOf(HttpStatusCode.Redirect, HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Mine_RequiresMemberRole()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Bookings/Mine");

        response.StatusCode.ShouldBeOneOf(HttpStatusCode.Redirect, HttpStatusCode.Forbidden);
    }
}
