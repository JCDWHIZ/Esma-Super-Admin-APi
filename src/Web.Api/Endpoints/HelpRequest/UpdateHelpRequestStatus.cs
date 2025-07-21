using Application.HelpRequest.UpdateHelpRequestStatus;
using Application.Abstractions.Messaging;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.HelpRequest;

namespace Web.Api.Endpoints.HelpRequest;

internal sealed class UpdateStatus : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("help-requests/{publicId:guid}/status", async (
            Guid publicId,
            HelpStatus status,
            ICommandHandler<UpdateHelpRequestStatusCommand, HelpRequestDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateHelpRequestStatusCommand
            {
                PublicId = publicId,
                Status = status
            };

            Result<HelpRequestDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.HelpRequests);
        // .RequireAuthorization();
    }
}
