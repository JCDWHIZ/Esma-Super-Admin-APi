using Application.School.CreateSchool;
using Application.Subscription.GetSubscriptionBySchoolId;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.Subscription;

public class UpdateSubscriptionBySchoolId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("subscriptions/{schoolPublicId:guid}", async (
            Guid schoolPublicId,
            IQueryHandler<GetSubscriptionBySchoolIdQuery, SubscriptionDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSubscriptionBySchoolIdQuery(schoolPublicId);

            Result<SubscriptionDto> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Subscription)
        .RequireAuthorization(new RequirePermissionAttribute("school_subscription_view"));
    }
}
