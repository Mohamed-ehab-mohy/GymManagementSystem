using System;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Repositories;

public interface IUnitOfWork : IDisposable
{
    Task<int> CompleteAsync();
}
