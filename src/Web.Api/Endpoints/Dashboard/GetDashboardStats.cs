
using Application.Dashboard;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.Dashboard;

public class GetDashboardStats : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("dashboard/stats", async (
            IQueryHandler<DashboardStatsQuery, DashboardStatsDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new DashboardStatsQuery();

            Result<DashboardStatsDto> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Dashboard)
        .Produces<DashboardStatsDto>(StatusCodes.Status200OK)
        .RequireAuthorization(new RequirePermissionAttribute("dashboard_view"));
    }
}
