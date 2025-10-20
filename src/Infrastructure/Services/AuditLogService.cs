using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Interfaces;
using Domain.AuditLogs;

namespace Infrastructure.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IApplicationDbContext _context;
    private readonly IUserContext _userContext;

    public AuditLogService(IApplicationDbContext context, IUserContext userContext)
    {
        _context = context;
        _userContext = userContext;
    }

    public async Task LogAsync(string actionDescription)
    {
        var auditLog = new AuditLog
        {
            Action = actionDescription,
            CreatedBy = _userContext.UserPublicId,
            Created = DateTimeOffset.UtcNow,
            Role = _userContext.UserRole ?? "unknown"
        };

        _context.Auditlog.Add(auditLog);
        await _context.SaveChangesAsync(default);
    }
}
