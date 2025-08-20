using System.Text.Json.Serialization;
namespace SharedKernel.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Roles
{
    ADMIN,
    BUSINESS_ADMIN,
    SYSTEM_ADMIN,
    USER_AND_ROLE_MANAGER,
    SECURITY_OFFICER,
    CONTENT_MEDIA_MANAGER,
    SUPPORT_MANAGER,
    REPORT_MANAGER,
}
