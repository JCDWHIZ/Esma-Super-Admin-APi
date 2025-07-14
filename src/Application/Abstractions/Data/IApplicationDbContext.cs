using Domain.AuditLogs;
using Domain.Blogs;
using Domain.HelpRequests;
using Domain.Schools;
using Domain.Subscriptions;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<TodoItem> TodoItems { get; }
    DbSet<Schools> Schools { get; }
    DbSet<Subscriptions> Subscriptions { get; }
    DbSet<HelpRequests> HelpRequests { get; }
    DbSet<HelpRequestMessages> HelpRequestMessages { get; }
    DbSet<Blog> Blog { get; }
    DbSet<AuditLog> Auditlog { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
