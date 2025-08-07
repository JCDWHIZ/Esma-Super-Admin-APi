using System.Text.Json.Serialization;
namespace SharedKernel.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]

public enum SubscriptionStatus
{
    Active,
    Expired,
    ExpiresSoon
}