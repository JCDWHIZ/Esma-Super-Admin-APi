using System.Text.Json.Serialization;
namespace SharedKernel.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PriorityLevel
{
    NONE,
    LOW,
    MEDIUM,
    HIGH
}