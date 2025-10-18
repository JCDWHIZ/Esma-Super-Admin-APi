using System.Text.Json.Serialization;
namespace SharedKernel.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TriggerType
{
    WELCOME_SCHOOLADMIN_EMAIL,
    WELCOME_ADMIN_EMAIL,
    PASSWORD_RESET,
    SUBSCRIPTION_EXPIRED
    //NOTIFICATION_EMAIL,
    //ACCOUNT_VERIFICATION
}
