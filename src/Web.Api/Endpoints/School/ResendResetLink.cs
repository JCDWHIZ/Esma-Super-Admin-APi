using Application.School.ResendSchoolResetLink;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.School;

internal sealed class ResendResetLink : IEndpoint
{
    public sealed class Request
    {
        public Guid SchoolPublicId { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/schools/resend-reset-link", async (Request request, ICommandHandler<ResendSchoolResetLinkCommand, string> handler, CancellationToken cancellationToken) =>
        {
            var command = new ResendSchoolResetLinkCommand(request.SchoolPublicId);
            Result<string> result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName("ResendSchoolResetLink")
        .WithTags(Tags.Schools)
        .Produces<string>(StatusCodes.Status200OK)
        .WithAudit("Resent School Reset Link")
        .RequireAuthorization(new RequirePermissionAttribute("school_create"));
    }
}
