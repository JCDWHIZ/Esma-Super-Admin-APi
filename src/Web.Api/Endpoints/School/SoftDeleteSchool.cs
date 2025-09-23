using System;
using Application.School.SoftDeleteSchool;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.School;

public class SoftDeleteSchool : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("schools/{publicId:guid}/soft-delete", async (
            Guid publicId,
            ICommandHandler<SoftDeleteSchoolCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new SoftDeleteSchoolCommand(publicId);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Schools)
        .Produces<string>(StatusCodes.Status200OK)
        .WithAudit("Deleted School Temporary")
        .RequireAuthorization(new RequirePermissionAttribute("DeleteSchool"));
    }
}
