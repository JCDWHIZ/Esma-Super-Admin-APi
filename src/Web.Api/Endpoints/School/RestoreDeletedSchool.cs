using System;
using Application.School.RestoreDeletedSchool;

namespace Web.Api.Endpoints.School;

internal sealed class RestoreDeletedSchool : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("schools/{publicId:guid}/restore", async (
            Guid publicId,
            ICommandHandler<RestoreDeletedSchoolCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RestoreDeletedSchoolCommand(publicId);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Schools);
        // .RequireAuthorization();
    }
}
