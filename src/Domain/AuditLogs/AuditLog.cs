using System;
using SharedKernel.Models;

namespace Domain.AuditLogs;

public class AuditLog : BaseAuditableEntity
{
    public required string Role { get; set; }
    public required string Action { get; set; }
}
