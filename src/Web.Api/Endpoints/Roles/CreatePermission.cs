//using System;
//using Application.Dashboard;
//using Application.Roles.CreatePermission;
//using Domain.Users;

//namespace Web.Api.Endpoints.Roles;

//internal sealed class CreatePermission : IEndpoint
//{
//    public void MapEndpoint(IEndpointRouteBuilder app)
//    {
//        app.MapPost("roles/permissions", async (
//                Request request,
//                ICommandHandler<CreatePermissionCommand, PermissionDto> handler,
//                CancellationToken cancellationToken) =>
//            {
//                var command = new CreatePermissionCommand
//                {
//                    Name = request.Name,
//                    Description = request.Description
//                };

//                Result<PermissionDto> result = await handler.Handle(command, cancellationToken);

//                return result.Match(Results.Ok, CustomResults.Problem);
//            })
//            .RequireAuthorization()
//            .WithAudit("A permission was created")
//            .Produces<PermissionDto>(StatusCodes.Status200OK)
//            .WithTags(Tags.Roles);
//    }

//    public sealed record Request(string Name, string Description);
//}
