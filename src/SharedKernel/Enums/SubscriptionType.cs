using System.Text.Json.Serialization;
namespace SharedKernel.Enums;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SubscriptionType
{
    PREMIUM,
    CUSTOMIZED,
    BASIC,
    STANDARD,
}
