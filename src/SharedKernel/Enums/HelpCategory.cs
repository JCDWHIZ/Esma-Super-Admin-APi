using System.Text.Json.Serialization;
namespace SharedKernel.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HelpCategory
{
    BUG_REPORT,
    TECHNICAL_ISSUE,
    OTHER
}
