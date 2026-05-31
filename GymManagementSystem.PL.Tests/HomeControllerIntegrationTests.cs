using System.Net;
using System.Threading.Tasks;
using GymManagementSystem.PL.Tests.Infrastructure;
using Shouldly;
using Xunit;

namespace GymManagementSystem.PL.Tests;

public class HomeControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public HomeControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Index_LoadsSuccessfully()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("Gymy");
    }

    [Fact]
    public async Task Error_LoadsSuccessfully()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Home/Error?statusCode=404");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("Page Not Found");
    }
}
