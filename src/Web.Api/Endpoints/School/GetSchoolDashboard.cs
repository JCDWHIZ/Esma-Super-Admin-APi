using System;
using Application.School.GetSchoolDashboard;

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
        .WithTags(Tags.Schools);
        // .RequireAuthorization(); // Remove if public dashboard access is allowed
    }
}
