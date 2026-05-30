using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.IO;

namespace GymManagementSystem.BLL.Export;

internal static class PdfWriter
{
    public static byte[] WriteToPdf<T>(
        IEnumerable<T> data,
        IEnumerable<ColumnDefinition<T>> columns,
        string title)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(1, Unit.Centimetre);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header()
                    .Text(string.IsNullOrWhiteSpace(title) ? "Report" : title)
                    .SemiBold()
                    .FontSize(20)
                    .FontColor(Colors.Blue.Medium);

                page.Content().PaddingTop(10).Table(table =>
                {
                    var colList = new List<ColumnDefinition<T>>(columns);

                    table.ColumnsDefinition(columnsDefinition =>
                    {
                        foreach (var col in colList)
                        {
                            columnsDefinition.RelativeColumn();
                        }
                    });

                    table.Header(header =>
                    {
                        foreach (var col in colList)
                        {
                            header.Cell()
                                  .Background(Colors.Grey.Lighten2)
                                  .Padding(5)
                                  .Text(col.HeaderName)
                                  .Bold();
                        }
                    });

                    foreach (var item in data)
                    {
                        foreach (var col in colList)
                        {
                            var value = col.ValueSelector(item);
                            string textValue = value?.ToString() ?? string.Empty;

                            table.Cell()
                                 .BorderBottom(1)
                                 .BorderColor(Colors.Grey.Lighten3)
                                 .Padding(5)
                                 .Text(textValue);
                        }
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        });

        using (var stream = new MemoryStream())
        {
            document.GeneratePdf(stream);
            return stream.ToArray();
        }
    }
}
