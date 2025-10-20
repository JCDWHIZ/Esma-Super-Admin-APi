using Application.School.CreateSchool;

namespace Application.Subscription.GetSubscriptionBySchoolId;

public sealed record GetSubscriptionBySchoolIdQuery(Guid PublicId) : IQuery<SubscriptionDto>;

