using ClosedXML.Excel;
using System.Collections.Generic;
using System.IO;

namespace GymManagementSystem.BLL.Export;

internal static class ExcelWriter
{
    public static byte[] WriteToExcel<T>(
        IEnumerable<T> data,
        IEnumerable<ColumnDefinition<T>> columns,
        string sheetName)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add(string.IsNullOrWhiteSpace(sheetName) ? "Report" : sheetName);

            int currentRow = 1;
            int currentColumn = 1;

            foreach (var col in columns)
            {
                worksheet.Cell(currentRow, currentColumn).Value = col.HeaderName;
                worksheet.Cell(currentRow, currentColumn).Style.Font.Bold = true;
                currentColumn++;
            }

            foreach (var item in data)
            {
                currentRow++;
                currentColumn = 1;

                foreach (var col in columns)
                {
                    var value = col.ValueSelector(item);
                    worksheet.Cell(currentRow, currentColumn).Value = value != null ? XLCellValue.FromObject(value) : Blank.Value;
                    currentColumn++;
                }
            }

            worksheet.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
        }
    }
}
