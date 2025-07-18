using System.Linq.Expressions;
using Application.Abstractions.Data;
using Domain.AuditLogs;
using Domain.Blogs;
using Domain.HelpRequests;
using Domain.Schools;
using Domain.Subscriptions;
using Domain.Todos;
using Domain.Users;
using Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SharedKernel;
using SharedKernel.Models;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<TodoItem> TodoItems { get; set; }

    public DbSet<Schools> Schools => Set<Schools>();
    public DbSet<Subscriptions> Subscriptions => Set<Subscriptions>();
    public DbSet<HelpRequests> HelpRequests => Set<HelpRequests>();
    public DbSet<Blog> Blog => Set<Blog>();
    public DbSet<HelpRequestMessages> HelpRequestMessages => Set<HelpRequestMessages>();
    public DbSet<AuditLog> Auditlog => Set<AuditLog>();
    public DbSet<SchoolAdmins> SchoolAdmins => Set<SchoolAdmins>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // When should you publish domain events?
        //
        // 1. BEFORE calling SaveChangesAsync
        //     - domain events are part of the same transaction
        //     - immediate consistency
        // 2. AFTER calling SaveChangesAsync
        //     - domain events are a separate transaction
        //     - eventual consistency
        //     - handlers can fail

        int result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync();

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        foreach (Microsoft.EntityFrameworkCore.Metadata.IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                ParameterExpression parameter = Expression.Parameter(entityType.ClrType, "e");
                MemberExpression property = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
                BinaryExpression condition = Expression.Equal(property, Expression.Constant(false));
                LambdaExpression lambda = Expression.Lambda(condition, parameter);

                entityType.SetQueryFilter(lambda);
            }
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.HasDefaultSchema(Schemas.Default);
    }

    /*
     Error:
    “An error was generated for warning ‘Microsoft.EntityFrameworkCore.Migrations.PendingModelChangesWarning’: The model for context ‘ApplicationDbContext’ changes each time it is built. … This is usually caused by dynamic values used in a ‘HasData’ call (e.g. new DateTime(), Guid.NewGuid()). Add a new migration and examine its contents to locate the cause, and replace the dynamic call with a static, hardcoded value.”
    */
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.ConfigureWarnings(warnings =>
            // Convert PendingModelChangesWarning into a log (or ignore entirely)
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    private async System.Threading.Tasks.Task PublishDomainEventsAsync()
    {
        // 1. Collect events without clearing
        var entities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(entry => entry.Entity.DomainEvents.Any())
            .Select(entry => entry.Entity)
            .ToList();

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // 2. Dispatch FIRST
        if (domainEvents.Any())
        {
            try
            {
                await domainEventsDispatcher.DispatchAsync(domainEvents);
            }
            catch (Exception ex)
            {
                // CRITICAL: Add proper logging here
                Console.WriteLine($"[DomainEvents] Dispatch failed: {ex}");
                throw;
            }
        }

        // 3. Clear events ONLY after successful dispatch
        entities.ForEach(e => e.ClearDomainEvents());
    }
}
