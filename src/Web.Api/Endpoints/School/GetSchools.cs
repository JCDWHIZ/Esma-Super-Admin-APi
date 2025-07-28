using System;
using Application;
using Application.School;
using Application.School.GetSchoolsWithPagination;

namespace Web.Api.Endpoints.School;

internal sealed class GetSchools : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("schools", async (
            string? schoolName,
            string? logoUrl,
            string? addressCountry,
            string? addressState,
            string? addressLga,
            string? addressStreetAddress,
            string? emailAddress,
            bool? subscribed,
            SchoolStatus? status,
            string? phoneNumber,
            SubscriptionType? subscriptionType,
            int? pageNumber,
            int? pageSize,
            IQueryHandler<GetSchoolsWithPaginationQuery, PaginatedList<SchoolItemDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSchoolsWithPaginationQuery
            {
                SchoolName = schoolName,
                LogoUrl = logoUrl,
                AddressCountry = addressCountry,
                AddressState = addressState,
                AddressLga = addressLga,
                AddressStreetAddress = addressStreetAddress,
                EmailAddress = emailAddress,
                Subscribed = subscribed,
                Status = status,
                PhoneNumber = phoneNumber,
                SubscriptionType = subscriptionType,
                PageNumber = pageNumber ?? 1,
                PageSize = pageSize ?? 10
            };

            Result<PaginatedList<SchoolItemDto>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Schools)
        .Produces<PaginatedList<SchoolItemDto>>()
        .RequireAuthorization();
    }
}