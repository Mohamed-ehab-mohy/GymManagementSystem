using System.Net;
using System.Threading.Tasks;
using GymManagementSystem.PL.Tests.Infrastructure;
using Shouldly;
using Xunit;

namespace GymManagementSystem.PL.Tests;

public class AccountControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AccountControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_Get_ReturnsSuccess()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Account/Login");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("Login");
    }

    [Fact]
    public async Task Register_Get_ReturnsSuccess()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Account/Register");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("Register");
    }

    [Fact]
    public async Task ForgotPassword_Get_ReturnsSuccess()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Account/ForgotPassword");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("Forgot");
    }
}
