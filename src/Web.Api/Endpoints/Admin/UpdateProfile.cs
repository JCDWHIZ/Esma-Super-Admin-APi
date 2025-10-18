using Application.Admin.UpdateProfile;

namespace Web.Api.Endpoints.Admin;

internal sealed class UpdateProfileEndpoint : IEndpoint
{
    public sealed class Request
    {
        public string Username { get; init; }
        public string PhoneNumber { get; init; }
        public string Email { get; init; }
        public string ProfilePic { get; init; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/admin/profile/{publicId:guid}", async (
            Guid publicId,
            Request request,
            ICommandHandler<UpdateProfileCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateProfileCommand(
                request.Username,
                request.PhoneNumber,
                request.Email,
                request.ProfilePic,
                publicId);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Admin)
        .RequireAuthorization();
    }
}
