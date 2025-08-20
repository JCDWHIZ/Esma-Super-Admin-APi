using Application.Admin;
using Application.Admin.EditAdmin;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Admin;

internal sealed class EditAdminEndpoint : IEndpoint
{
    public sealed class Request
    {
        public string Email { get; init; }
        public string FirstName { get; init; }
        public string? ProfilePic { get; init; }
        public string? PhoneNumber { get; init; }
        public string LastName { get; init; }
        public SharedKernel.Enums.Roles Role { get; init; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/admins/{publicId}", async (
            Guid publicId,
            Request request,
            ICommandHandler<EditAdminCommand, UserDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new EditAdminCommand
            {
                PublicId = publicId,
                Email = request.Email,
                FirstName = request.FirstName,
                ProfilePic = request.ProfilePic,
                PhoneNumber = request.PhoneNumber,
                LastName = request.LastName,
                Role = request.Role
            };

            Result<UserDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .Produces<UserDto>(StatusCodes.Status200OK)
        .WithAudit("An admin's details were edited")
        .RequireAuthorization();
    }
}
