using System;
using Application.School.ApproveSchool;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.School;

internal sealed class ApproveSchool : IEndpoint
{
    public sealed class Request {
        public List<Guid> PublicIds { get; set; }
    }
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/schools/approve", async (Request request, ICommandHandler<ApproveSchoolCommand, string> handler, CancellationToken cancellationToken) =>
        {
            var command = new ApproveSchoolCommand(request.PublicIds);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName("ApproveSchool")
        .WithTags(Tags.Schools)
        .Produces<string>(StatusCodes.Status200OK)
        .WithAudit("Approved School")
        .RequireAuthorization(new RequirePermissionAttribute("school_approve"));
    }
}
