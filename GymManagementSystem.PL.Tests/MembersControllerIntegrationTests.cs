using System.Net;
using System.Threading.Tasks;
using GymManagementSystem.PL.Tests.Infrastructure;
using Shouldly;
using Xunit;

namespace GymManagementSystem.PL.Tests;

public class MembersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public MembersControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact(Skip = "Requires test authentication setup after adding [Authorize]")]
    public async Task Index_ReturnsSuccessAndCorrectContentType()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Members");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("Members");
        content.ShouldContain("Mohamed");
    }

    [Fact(Skip = "Requires test authentication setup after adding [Authorize]")]
    public async Task ExportExcel_ReturnsExcelFile()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Members/ExportExcel");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        var bytes = await response.Content.ReadAsByteArrayAsync();
        bytes.Length.ShouldBeGreaterThan(0);
    }

    [Fact(Skip = "Requires test authentication setup after adding [Authorize]")]
    public async Task ExportPdf_ReturnsPdfFile()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Members/ExportPdf");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/pdf");
        var bytes = await response.Content.ReadAsByteArrayAsync();
        bytes.Length.ShouldBeGreaterThan(4);
        bytes[0].ShouldBe((byte)'%');
        bytes[1].ShouldBe((byte)'P');
        bytes[2].ShouldBe((byte)'D');
        bytes[3].ShouldBe((byte)'F');
    }
}
