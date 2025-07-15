using System;

namespace Application.AuditLogModule;

public class AuditLogDto
{
    public string? Role { get; set; } = string.Empty;
    public string? Action { get; set; } = string.Empty;
}
