using System;
using Application;
using Application.School.CreateSchool;
using Application.Subscription.GetSubscriptionWithPagination;

namespace Web.Api.Endpoints.Subscription;

public sealed class GetSubscriptions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("subscriptions", async (
            SubscriptionType? subscriptionType,
            DateTime? startDate,
            DateTime? endDate,
            decimal? amount,
            string? schoolName,
            int? pageNumber,
            int? pageSize,
            IQueryHandler<GetSubscriptionWithPaginationQuery, PaginatedList<SubscriptionDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSubscriptionWithPaginationQuery
            {
                SubscriptionType = subscriptionType,
                StartDate = startDate,
                EndDate = endDate,
                Amount = amount,
                SchoolName = schoolName ?? string.Empty,
                PageNumber = pageNumber ?? 1,
                PageSize = pageSize ?? 10
            };

            Result<PaginatedList<SubscriptionDto>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Subscription)
        .RequireAuthorization();
    }
}
