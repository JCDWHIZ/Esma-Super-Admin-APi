using System.Text.Json.Serialization;

namespace Application.Abstractions.Models;

public enum TenantUpdateAction
{
    [JsonStringEnumMemberName("ACTIVATE")]
    ACTIVATE,
    [JsonStringEnumMemberName("DEACTIVATE")]
    DEACTIVATE
}

public class UpdateTenantStatusMessage
{
    public string SchoolPublicId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public TenantUpdateAction Action { get; set; }
}
