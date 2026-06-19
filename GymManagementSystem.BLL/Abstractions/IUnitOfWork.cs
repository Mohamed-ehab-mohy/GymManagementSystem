using System;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Abstractions;

public interface IUnitOfWork : IDisposable
{
    Task<int> CompleteAsync();
}
