using System.Text.Json.Serialization;
namespace SharedKernel.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Roles
{
    ADMIN,
    MANAGER,
    CONTENT_CREATOR,
    SCHOOL_SUPER_ADMIN
}
