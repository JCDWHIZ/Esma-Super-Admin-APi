namespace Application.Abstractions.Models;

public class TenantStatusUpdatedResponse
{
    public required string SchoolPublicId { get; set; }
    public required string TenantId { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public TenantUpdateAction Action { get; set; }
}
