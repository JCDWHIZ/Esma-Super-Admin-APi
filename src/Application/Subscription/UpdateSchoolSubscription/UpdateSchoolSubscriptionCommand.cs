namespace Application.Subscription.UpdateSchoolSubscription;

public sealed record UpdateSchoolSubscriptionCommand(
    Guid SchoolId,
    SubscriptionType SubscriptionType,
    DateTime? StartDate,
    DateTime? EndDate,
    decimal Amount,
    ICollection<string> Modules
) : ICommand<string>;
