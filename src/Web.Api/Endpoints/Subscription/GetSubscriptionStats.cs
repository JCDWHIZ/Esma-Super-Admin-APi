using System;
using Application.Subscription.GetSubscriptionStats;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.Subscription;

internal sealed class GetSubscriptionStats : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("subscriptions/stats", async (
            ICommandHandler<GetSubscriptionStatsQuery, SubscriptionStatsDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new GetSubscriptionStatsQuery();

            Result<SubscriptionStatsDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Subscription)
        .Produces<SubscriptionStatsDto>(StatusCodes.Status200OK)
        .RequireAuthorization(new RequirePermissionAttribute("school_subscription_view"));
    }
}
