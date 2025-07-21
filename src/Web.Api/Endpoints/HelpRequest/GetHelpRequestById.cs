using Application.HelpRequest.GetHelpRequestById;
using Application.Abstractions.Messaging;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.HelpRequest;

namespace Web.Api.Endpoints.HelpRequest;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("help-requests/{publicId:guid}", async (
            Guid publicId,
            IQueryHandler<GetHelpRequestByIdQuery, HelpRequestDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetHelpRequestByIdQuery(publicId);

            Result<HelpRequestDto> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.HelpRequests);
        // .RequireAuthorization();
    }
}
