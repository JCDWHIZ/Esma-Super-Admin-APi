using System;
using Application.Dashboard;
using Application.Roles.CreateRole;
using Domain.Users;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.Roles;

internal sealed class CreateRole : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("roles", async (
                Request request,
                ICommandHandler<CreateRoleCommand, string> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateRoleCommand
                {
                    Name = request.Name,
                    Description = request.Description,
                    IsDefault = request.IsDefault,
                    PermissionIds = request.PermissionIds
                };

                Result<string> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .RequireAuthorization(new RequirePermissionAttribute("role_create"))
            .WithAudit("A role was created")
            .Produces<RoleDto>(StatusCodes.Status200OK)
            .WithTags(Tags.Roles);
    }

    public sealed record Request(string Name, string Description, ICollection<Guid> PermissionIds, bool IsDefault);
}
