using System;
using Application.School.GetSchoolStats;

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
        .WithTags(Tags.Schools);
        // .RequireAuthorization(); // Remove if stats should be public
    }
}
