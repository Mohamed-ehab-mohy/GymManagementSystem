using System;

namespace GymManagementSystem.BLL.Export;

public class ColumnDefinition<T>
{
    public string HeaderName { get; set; } = string.Empty;
    public Func<T, object> ValueSelector { get; set; } = null!;
}
