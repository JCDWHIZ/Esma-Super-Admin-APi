using Domain.AuditLogs;
using Domain.Blogs;
using Domain.HelpRequests;
using Domain.Schools;
using Domain.Subscriptions;
using Domain.Templates;
using Domain.Todos;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<TodoItem> TodoItems { get; }
    DbSet<Schools> Schools { get; }
    DbSet<Subscriptions> Subscriptions { get; }
    DbSet<HelpRequests> HelpRequests { get; }
    DbSet<Domain.Roles.Role> Roles { get; }
    DbSet<Domain.Roles.Permission> Permissions { get; }
    DbSet<HelpRequestMessages> HelpRequestMessages { get; }
    DbSet<Blog> Blog { get; }
    DbSet<AuditLog> Auditlog { get; }
    DbSet<SchoolAdmins> SchoolAdmins { get; }
    DbSet<Template> Templates { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
