using System.Net;
using System.Net.Http;
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

    [Fact]
    public async Task Index_ReturnsSuccessAndContainsTitle()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Members");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("Members");
    }

    [Fact]
    public async Task DataTableData_ReturnsJsonWithMembers()
    {
        var client = _factory.CreateClient();

        var formData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("draw", "1"),
            new KeyValuePair<string, string>("start", "0"),
            new KeyValuePair<string, string>("length", "10"),
            new KeyValuePair<string, string>("search[value]", ""),
            new KeyValuePair<string, string>("order[0][column]", "0"),
            new KeyValuePair<string, string>("order[0][dir]", "asc"),
            new KeyValuePair<string, string>("columns[0][data]", "name"),
            new KeyValuePair<string, string>("columns[0][name]", "FirstName"),
            new KeyValuePair<string, string>("columns[1][data]", "contact"),
            new KeyValuePair<string, string>("columns[1][name]", ""),
            new KeyValuePair<string, string>("columns[2][data]", "city"),
            new KeyValuePair<string, string>("columns[2][name]", ""),
            new KeyValuePair<string, string>("columns[3][data]", "actions"),
            new KeyValuePair<string, string>("columns[3][name]", "")
        });

        var response = await client.PostAsync("/Members/DataTableData", formData);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("draw");
        content.ShouldContain("recordsTotal");
        content.ShouldContain("data");
    }

    [Fact]
    public async Task ExportExcel_ReturnsExcelFile()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Members/ExportExcel");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        var bytes = await response.Content.ReadAsByteArrayAsync();
        bytes.Length.ShouldBeGreaterThan(0);
    }

    [Fact]
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
