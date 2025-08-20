using System;
using Application.Dashboard;
using Application.Roles.CreateRole;
using Domain.Users;

namespace Web.Api.Endpoints.Roles;

internal sealed class CreateRole : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("roles", async (
                Request request,
                ICommandHandler<CreateRoleCommand, RoleDto> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateRoleCommand
                {
                    Name = request.Name,
                    Description = request.Description
                };

                Result<RoleDto> result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithAudit("A role was created")
            .Produces<RoleDto>(StatusCodes.Status200OK)
            .WithTags(Tags.Roles);
    }

    public sealed record Request(string Name, string Description);
}
