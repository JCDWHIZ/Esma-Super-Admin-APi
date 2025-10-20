namespace Application.AuditLogModule;

public class AuditLogDto
{
    public string? Role { get; set; } = string.Empty;
    public string? Action { get; set; } = string.Empty;
    public DateTimeOffset? CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? PublicId { get; set; }
}
