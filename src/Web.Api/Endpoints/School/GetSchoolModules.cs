using Application.School;
using Application.School.GetSchoolModules;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.School;

public class GetSchoolModules : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("schools/modules", async (
            IQueryHandler<GetSchoolModulesQuery, IReadOnlyList<SchoolModuleResponseDto>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<SchoolModuleResponseDto>> result =
                await handler.Handle(new GetSchoolModulesQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Schools)
        .Produces<IReadOnlyList<SchoolModuleResponseDto>>(StatusCodes.Status200OK)
        .RequireAuthorization(new RequirePermissionAttribute("school_view"));
    }
}
