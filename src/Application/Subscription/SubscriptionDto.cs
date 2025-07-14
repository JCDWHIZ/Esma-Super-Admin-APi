using System;
using admin_service.Domain.Entities;
using admin_service.Domain.Enums;

namespace admin_service.Application.Subscription.Queries;

public class SubscriptionDto
{
    public string PublicId { get; set; } = string.Empty;
    public int Id { get; set; }
    public SubscriptionType SubscriptionType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Amount { get; set; }
    public int SchoolId { get; set; }
    // public string SchoolName { get; set; } = string.Empty;
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Subscriptions, SubscriptionDto>();
        }
    }
}
