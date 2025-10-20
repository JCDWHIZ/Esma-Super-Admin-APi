using SharedKernel.Models;

namespace Domain.AuditLogs;

public sealed class AuditLog : BaseAuditableEntity
{
    public required string Role { get; set; }
    public required string Action { get; set; }
}
