using Application.School.GetSchoolStats;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.School;

internal sealed class GetSchoolStats : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("schools/stats", async (
            IQueryHandler<GetSchoolStatsQuery, SchoolStatsCountDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSchoolStatsQuery();

            Result<SchoolStatsCountDto> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Schools)
        .Produces<SchoolStatsCountDto>(StatusCodes.Status200OK)
        .RequireAuthorization(new RequirePermissionAttribute("school_view"));
    }
}
