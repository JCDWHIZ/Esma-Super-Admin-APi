using Application.School;
using Application.School.GetLmsModules;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.School;

public class GetLmsModules : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("schools/lms-modules", async (
            IQueryHandler<GetLmsModulesQuery, IReadOnlyList<LmsModuleResponseDto>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<LmsModuleResponseDto>> result =
                await handler.Handle(new GetLmsModulesQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Schools)
        .Produces<IReadOnlyList<LmsModuleResponseDto>>(StatusCodes.Status200OK)
        .RequireAuthorization(new RequirePermissionAttribute("school_view"));
    }
}
