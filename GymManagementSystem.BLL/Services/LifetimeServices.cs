using GymManagementSystem.BLL.Interfaces;

namespace GymManagementSystem.BLL.Services;

public class ScopedService : IScopedService
{
    public Guid OperationId { get; } = Guid.NewGuid();
}

public class TransientService : ITransientService
{
    public Guid OperationId { get; } = Guid.NewGuid();
}

public class SingletonService : ISingletonService
{
    public Guid OperationId { get; } = Guid.NewGuid();
}
