namespace Application.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(string actionDescription);
}
