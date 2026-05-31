using System.Collections.Generic;
using System.Threading.Tasks;
using GymManagementSystem.BLL.Export;
using Shouldly;
using Xunit;

namespace GymManagementSystem.BLL.Tests;

public class ExportServiceTests
{
    public class DummyItem
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    [Fact]
    public async Task ExportAsync_Excel_WithData_ReturnsNonEmptyBytes()
    {
        var service = new ExportService();
        var data = new List<DummyItem>
        {
            new() { Name = "John", Age = 30 },
            new() { Name = "Jane", Age = 25 }
        };
        var columns = new List<ColumnDefinition<DummyItem>>
        {
            new() { HeaderName = "Name", ValueSelector = x => x.Name },
            new() { HeaderName = "Age", ValueSelector = x => x.Age }
        };

        var bytes = await service.ExportAsync(data, columns, ExportFormat.Excel, "Test Excel");

        bytes.ShouldNotBeNull();
        bytes.Length.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task ExportAsync_Pdf_WithData_ReturnsPdfWithMagicBytes()
    {
        var service = new ExportService();
        var data = new List<DummyItem>
        {
            new() { Name = "John", Age = 30 }
        };
        var columns = new List<ColumnDefinition<DummyItem>>
        {
            new() { HeaderName = "Name", ValueSelector = x => x.Name }
        };

        var bytes = await service.ExportAsync(data, columns, ExportFormat.Pdf, "Test PDF");

        bytes.ShouldNotBeNull();
        bytes.Length.ShouldBeGreaterThan(4);
        bytes[0].ShouldBe((byte)'%');
        bytes[1].ShouldBe((byte)'P');
        bytes[2].ShouldBe((byte)'D');
        bytes[3].ShouldBe((byte)'F');
    }

    [Fact]
    public async Task ExportAsync_ExcelAndPdf_WithEmptyData_ReturnsValidBytes()
    {
        var service = new ExportService();
        var data = new List<DummyItem>();
        var columns = new List<ColumnDefinition<DummyItem>>
        {
            new() { HeaderName = "Name", ValueSelector = x => x.Name }
        };

        var excelBytes = await service.ExportAsync(data, columns, ExportFormat.Excel, "Empty");
        var pdfBytes = await service.ExportAsync(data, columns, ExportFormat.Pdf, "Empty");

        excelBytes.ShouldNotBeNull();
        excelBytes.Length.ShouldBeGreaterThan(0);
        pdfBytes.ShouldNotBeNull();
        pdfBytes.Length.ShouldBeGreaterThan(4);
        pdfBytes[0].ShouldBe((byte)'%');
        pdfBytes[1].ShouldBe((byte)'P');
        pdfBytes[2].ShouldBe((byte)'D');
        pdfBytes[3].ShouldBe((byte)'F');
    }

    [Fact]
    public async Task ExportAsync_Excel_VerifyCustomHeaderAndValueSelection()
    {
        var service = new ExportService();
        var data = new List<DummyItem>
        {
            new() { Name = "TestName", Age = 99 }
        };
        var columns = new List<ColumnDefinition<DummyItem>>
        {
            new() { HeaderName = "CustomHeader", ValueSelector = x => x.Name.ToUpper() }
        };

        var bytes = await service.ExportAsync(data, columns, ExportFormat.Excel, "Custom");

        bytes.ShouldNotBeNull();
        bytes.Length.ShouldBeGreaterThan(0);
    }
}
