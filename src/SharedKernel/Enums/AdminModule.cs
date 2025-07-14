
using System.Text.Json.Serialization;
namespace SharedKernel.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AdminModule
{
    USER,
    // AUDIT_LOG,
    SCHOOL,
    BLOG,
}
