using System;
using Application.School.CreateSchool;
using Application.Subscription.GetSubscriptionBySchoolId;

namespace Web.Api.Endpoints.Subscription;

public class UpdateSubscriptionBySchoolId : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("subscriptions/by-school-id", async (
            Guid PublicId,
            IQueryHandler<GetSubscriptionBySchoolIdQuery, SubscriptionDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSubscriptionBySchoolIdQuery(PublicId);

            Result<SubscriptionDto> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Subscription)
        .RequireAuthorization();
    }
}
