using Application.Roles.EditRole;

namespace Web.Api.Endpoints.Roles;

internal sealed class EditRoleEndpoint : IEndpoint
{
    public sealed class Request
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public bool IsDefault { get; init; }
        public List<Guid> PermissionIds { get; init; } = new List<Guid>();
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/roles/{rolePublicId:guid}", async (
            Guid rolePublicId,
            Request request,
            ICommandHandler<EditRoleCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new EditRoleCommand
            {
                RolePublicId = rolePublicId,
                Name = request.Name,
                Description = request.Description,
                IsDefault = request.IsDefault,
                PermissionIds = request.PermissionIds
            };

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Roles)
        .RequireAuthorization();
    }
}
