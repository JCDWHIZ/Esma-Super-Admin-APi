using System;

namespace admin_service.Application.Common.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(string actionDescription);
}
