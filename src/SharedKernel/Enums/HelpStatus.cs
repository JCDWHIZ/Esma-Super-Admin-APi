using System.Text.Json.Serialization;
namespace SharedKernel.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HelpStatus
{
    OPEN_REQUEST,
    IN_PROGRESS,
    RESOLVED
}
