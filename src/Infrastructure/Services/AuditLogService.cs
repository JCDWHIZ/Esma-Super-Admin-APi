using System;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Application.Abstractions.Data;
using Domain.AuditLogs;

namespace Infrastructure.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IApplicationDbContext _context;

    public AuditLogService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(string actionDescription)
    {
        var auditLog = new AuditLog
        {
            Action = actionDescription,
            CreatedBy = Guid.Empty,
            Created = DateTimeOffset.UtcNow,
            Role = "Unknown"
        };

        _context.Auditlog.Add(auditLog);
        await _context.SaveChangesAsync(default);
    }
}