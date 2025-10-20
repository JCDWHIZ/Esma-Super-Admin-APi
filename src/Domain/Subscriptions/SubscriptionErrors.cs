using SharedKernel;

namespace Domain.Subscriptions;

public static class SubscriptionErrors
{
    public static Error NotFound(Guid publicId) => Error.NotFound(
        "Subscription.NotFound",
        $"The subscription with the PublicId = '{publicId}' was not found.");

    public static Error NotFoundForSchool(int schoolId) => Error.NotFound(
        "Subscription.NotFoundForSchool",
        $"No subscription found for school with Id = '{schoolId}'.");

    public static readonly Error AlreadyExpired = Error.Conflict(
        "Subscription.AlreadyExpired",
        "The subscription has already expired.");

    public static readonly Error AlreadyActive = Error.Conflict(
        "Subscription.AlreadyActive",
        "The subscription is already active.");

    public static readonly Error InvalidDateRange = Error.Failure(
        "Subscription.InvalidDateRange",
        "The end date must be after the start date.");

    public static readonly Error InvalidAmount = Error.Failure(
        "Subscription.InvalidAmount",
        "The subscription amount must be greater than zero.");

    public static readonly Error CannotCancelExpired = Error.Conflict(
        "Subscription.CannotCancelExpired",
        "Cannot cancel an already expired subscription.");

    public static readonly Error PaymentRequired = Error.Failure(
        "Subscription.PaymentRequired",
        "Payment is required to activate the subscription.");
}