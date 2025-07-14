using System;
using System.Text.Json.Serialization;
using SharedKernel.Enums;
using SharedKernel.Models;

namespace Domain.Subscriptions;

public class Subscriptions : BaseAuditableEntity
{
    public SubscriptionType SubscriptionType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Amount { get; set; }
    public int SchoolId { get; set; }
    [JsonIgnore]
    public Schools.Schools? Schools { get; set; }
}
