using System.Text.Json.Serialization;
namespace SharedKernel.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BlogStatus
{
    DRAFT,
    PUBLISHED,
    SCHEDULED,
    PENDING_APPROVAL,
}