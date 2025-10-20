using System.Text.Json.Serialization;
using SharedKernel.Enums;
using SharedKernel.Models;

namespace Domain.Subscriptions;

public sealed class Subscriptions : BaseAuditableEntity
{
    public SubscriptionType SubscriptionType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Amount { get; set; }
    public int SchoolId { get; set; }
    [JsonIgnore]
    public Schools.Schools? Schools { get; set; }
    public static SubscriptionStatus GetStatus(DateTime? startDate, DateTime? endDate)
    {
        DateTime now = DateTime.UtcNow;

        if (!startDate.HasValue || !endDate.HasValue)
        {
            return SubscriptionStatus.Expired;
        }

        if (endDate.Value < now)
        {
            return SubscriptionStatus.Expired;
        }

        if (endDate.Value <= now.AddDays(7))
        {
            return SubscriptionStatus.ExpiresSoon;
        }

        return SubscriptionStatus.Active;
    }
}
