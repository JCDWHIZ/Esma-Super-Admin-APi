using Application.Roles.GetRoleById;
using Domain.Users;

namespace Web.Api.Endpoints.Roles;

internal sealed class GetRoleByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/roles/{rolePublicId:guid}", async (
            Guid rolePublicId,
            IQueryHandler<GetRoleByIdQuery, RolesDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetRoleByIdQuery(rolePublicId);

            Result<RolesDto> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Roles)
        .Produces<RolesDto>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}
