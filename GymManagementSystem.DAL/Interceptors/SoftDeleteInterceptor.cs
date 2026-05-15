using GymManagementSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Interceptors;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        HandleSoftDelete(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        HandleSoftDelete(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Intercepts entity deletions and converts them to soft-updates if the entity has an IsDeleted property.
    /// Also protects owned entities from being marked as deleted.
    /// </summary>
    private void HandleSoftDelete(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Deleted && 
                entry.Metadata.FindProperty("IsDeleted") != null)
            {
                entry.State = EntityState.Unchanged;

                entry.Property("IsDeleted").CurrentValue = true;
                entry.Property("IsDeleted").IsModified = true;

                if (entry.Metadata.FindProperty("DeletedAt") != null)
                {
                    entry.Property("DeletedAt").CurrentValue = System.DateTime.UtcNow;
                    entry.Property("DeletedAt").IsModified = true;
                }

                if (entry.Metadata.FindProperty("UpdatedAt") != null)
                {
                    entry.Property("UpdatedAt").CurrentValue = System.DateTime.UtcNow;
                    entry.Property("UpdatedAt").IsModified = true;
                }
            }
            else if (entry.State == EntityState.Deleted && entry.Metadata.IsOwned())
            {
                entry.State = EntityState.Unchanged;
            }
        }
    }
}
