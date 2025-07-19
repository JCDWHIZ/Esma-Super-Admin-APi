using System;
using Application.Admin;
using Application.Admin.InviteAdmin;



namespace Web.Api.Endpoints.Admin;

internal sealed class InviteAdmin : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/admin/invite", async (Request request, ICommandHandler<InviteAdminCommand, UserDto> handler, CancellationToken cancellationToken) =>
        {
            var command = new InviteAdminCommand
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = request.Role
            };

            Result<UserDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName("InviteAdmin")
        .WithTags(Tags.Admin);
    }

    public sealed class Request
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Roles Role { get; set; }
    }

}
