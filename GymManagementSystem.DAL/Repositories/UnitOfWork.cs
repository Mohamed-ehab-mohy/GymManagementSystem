using GymManagementSystem.DAL.DbContexts;
using GymManagementSystem.BLL.Abstractions; using System;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly GymDbContext _context;
    private bool _disposed;

    public UnitOfWork(GymDbContext context)
    {
        _context = context;
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
