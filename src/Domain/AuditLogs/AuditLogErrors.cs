using SharedKernel;

namespace Domain.AuditLogs;

public static class AuditLogErrors
{
    public static Error NotFound(Guid publicId) => Error.NotFound(
        "AuditLog.NotFound",
        $"The audit log with the PublicId = '{publicId}' was not found.");

    public static readonly Error InvalidRole = Error.Failure(
        "AuditLog.InvalidRole",
        "The specified role is not valid.");

    public static readonly Error InvalidAction = Error.Failure(
        "AuditLog.InvalidAction",
        "The specified action is not valid.");

    public static readonly Error CreationFailed = Error.Failure(
        "AuditLog.CreationFailed",
        "Failed to create the audit log entry.");
}