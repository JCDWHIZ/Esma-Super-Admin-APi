using System;
using Application.Admin;
using Application.Admin.InviteAdmin;
using Infrastructure.Authorization;



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
                Role = request.Role,
                ProfilePic = request.ProfilePic,
                PhoneNumber = request.PhoneNumber
            };

            Result<UserDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName("InviteAdmin")
        .WithTags(Tags.Admin)
        .Produces<UserDto>(StatusCodes.Status200OK)
        .RequireAuthorization(new RequirePermissionAttribute("ViewAdmins"));
    }

    public sealed class Request
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string? ProfilePic { get; set; }
        public string? PhoneNumber { get; set; }
        public string LastName { get; set; }
        public SharedKernel.Enums.Roles Role { get; set; }
    }

}
