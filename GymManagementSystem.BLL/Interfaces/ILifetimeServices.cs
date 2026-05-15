namespace GymManagementSystem.BLL.Interfaces;

public interface IScopedService
{
    Guid OperationId { get; }
}

public interface ITransientService
{
    Guid OperationId { get; }
}

public interface ISingletonService
{
    Guid OperationId { get; }
}
