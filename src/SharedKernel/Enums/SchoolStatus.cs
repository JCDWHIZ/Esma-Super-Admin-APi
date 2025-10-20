using System.Text.Json.Serialization;
namespace SharedKernel.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SchoolStatus
{
    REJECTED,
    APPROVED,
    PENDING,
    ACTIVE,
    INACTIVE,
    SUSPENDED,
    PROCESSING
}
