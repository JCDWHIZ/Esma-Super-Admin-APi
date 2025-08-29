using System;

namespace Application.AuditLogModule.GetAuditLogs;

public record GetAuditLogsQuery : IQuery<PaginatedList<AuditLogDto>>
{
    public string? SearchTerm { get; set; }
    public string? Action { get; set; }
    public string? Role { get; set; }
    public TimeFilter? TimeFilter { get; set; }
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
}
