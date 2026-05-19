using Application.Subscription.UpdateSchoolSubscription;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.Subscription;

public class UpdateSubscriptionById : IEndpoint
{
    public sealed class Request
    {
        public Guid SchoolId { get; init; }
        public SubscriptionType SubscriptionType { get; init; }
        public DateTime? StartDate { get; init; }
        public DateTime? EndDate { get; init; }
        public decimal Amount { get; init; }
        public ICollection<string> Modules { get; init; } = [];
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("subscriptions/schools", async (
            Request request,
            ICommandHandler<UpdateSchoolSubscriptionCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateSchoolSubscriptionCommand(
                request.SchoolId,
                request.SubscriptionType,
                request.StartDate,
                request.EndDate,
                request.Amount,
                request.Modules
            );

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Subscription)
        .WithAudit("Subscription details was just updated for a school")
        .Produces<string>()
        .RequireAuthorization(new RequirePermissionAttribute("school_subscription_edit"));
    }
}
