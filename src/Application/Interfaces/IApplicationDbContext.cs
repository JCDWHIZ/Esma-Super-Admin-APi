using admin_service.Domain.Entities;

namespace admin_service.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoList> TodoLists { get; }
    DbSet<TodoItem> TodoItems { get; }
    DbSet<Schools> Schools { get; }
    DbSet<Subscriptions> Subscriptions { get; }
    DbSet<HelpRequests> HelpRequests { get; }
    DbSet<HelpRequestMessages> HelpRequestMessages { get; }
    DbSet<Blog> Blog { get; }
    DbSet<User> Users { get; }
    DbSet<AuditLog> Auditlog { get; }
    DbSet<T> Set<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
