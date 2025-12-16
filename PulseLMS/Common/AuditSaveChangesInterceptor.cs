using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PulseLMS.Domain.Entities;

namespace PulseLMS.Common;

public sealed class AuditSaveChangesInterceptor(ICurrentUser currentUser) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ApplyAudit(eventData.Context);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyAudit(eventData.Context);
        return ValueTask.FromResult(result);
    }

    private void ApplyAudit(DbContext? db)
    {
        if (db is null) return;

        var now = DateTimeOffset.UtcNow;
        var userId = currentUser.UserId; // Guid?

        foreach (var entry in db.ChangeTracker.Entries<AuditLog>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.UpdatedBy = userId;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = userId;

                    entry.Property(x => x.CreatedAt).IsModified = false;
                    entry.Property(x => x.CreatedBy).IsModified = false;
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                case EntityState.Deleted:
                    break;
                default:
                    var exception = new ArgumentOutOfRangeException
                    {
                        HelpLink = null,
                        HResult = 0,
                        Source = null
                    };
                   throw exception;
            }
        }
    }
}
