using System;
using admin_service.Application.Common.Interfaces;
using admin_service.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace admin_service.Infrastructure.Services;

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
                CreatedBy = "Unknown",
                Created = DateTimeOffset.UtcNow,
                Role = "Unknown"
            };

            _context.Auditlog.Add(auditLog);
            await _context.SaveChangesAsync(default);
    }
}