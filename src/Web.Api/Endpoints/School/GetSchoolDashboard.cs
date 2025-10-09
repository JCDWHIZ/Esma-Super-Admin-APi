using System;
using Application.School.GetSchoolDashboard;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.School;

internal sealed class GetSchoolDashboard : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("schools/dashboard", async (
            IQueryHandler<GetSchoolDashboardQuery, List<YearlyOverviewDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSchoolDashboardQuery();

            Result<List<YearlyOverviewDto>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Schools)
        .Produces<List<YearlyOverviewDto>>()
        .RequireAuthorization(new RequirePermissionAttribute("school_view"));
    }
}
