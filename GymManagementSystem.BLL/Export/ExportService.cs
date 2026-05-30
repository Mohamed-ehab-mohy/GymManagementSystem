using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Export;

public class ExportService : IExportService
{
    public Task<byte[]> ExportAsync<T>(
        IEnumerable<T> data,
        IEnumerable<ColumnDefinition<T>> columns,
        ExportFormat format,
        string title)
    {
        byte[] result;

        switch (format)
        {
            case ExportFormat.Excel:
                result = ExcelWriter.WriteToExcel(data, columns, title);
                break;

            case ExportFormat.Pdf:
                result = PdfWriter.WriteToPdf(data, columns, title);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(format), $"Format '{format}' is not supported.");
        }

        return Task.FromResult(result);
    }
}
