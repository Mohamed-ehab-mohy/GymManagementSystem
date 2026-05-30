using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Export;

public interface IExportService
{
    Task<byte[]> ExportAsync<T>(
        IEnumerable<T> data,
        IEnumerable<ColumnDefinition<T>> columns,
        ExportFormat format,
        string title);
}
