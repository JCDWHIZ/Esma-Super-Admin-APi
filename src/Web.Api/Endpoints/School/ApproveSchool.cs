using System;
using Application.School.ApproveSchool;

namespace Web.Api.Endpoints.School;

internal sealed class ApproveSchool : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/schools/approve/{PublicId:guid}", async (Guid PublicId, ICommandHandler<ApproveSchoolCommand, string> handler, CancellationToken cancellationToken) =>
        {
            var command = new ApproveSchoolCommand(PublicId);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName("ApproveSchool")
        .WithTags(Tags.Schools)
        .RequireAuthorization();
    }
}
