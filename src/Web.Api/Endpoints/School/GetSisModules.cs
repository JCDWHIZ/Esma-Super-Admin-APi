using Application.School;
using Application.School.GetSisModules;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.School;

public class GetSisModules : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("schools/sis-modules", async (
            IQueryHandler<GetSisModulesQuery, IReadOnlyList<SisModuleResponseDto>> handler,
            CancellationToken cancellationToken) =>
        {
            Result<IReadOnlyList<SisModuleResponseDto>> result =
                await handler.Handle(new GetSisModulesQuery(), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Schools)
        .Produces<IReadOnlyList<SisModuleResponseDto>>(StatusCodes.Status200OK)
        .RequireAuthorization(new RequirePermissionAttribute("school_view"));
    }
}
