using Application.HelpRequest.CreateHelpRequest;
using Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.HelpRequest;

namespace Web.Api.Endpoints.HelpRequest;

internal sealed class CreateHelpRequest : IEndpoint
{
    public sealed class Request
    {
        public string? TicketId { get; set; }
        public HelpStatus Status { get; set; } = HelpStatus.OPEN_REQUEST;
        public HelpCategory Category { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("help-requests", async (
            Request request,
            ICommandHandler<CreateHelpRequestCommand, HelpRequestDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateHelpRequestCommand
            {
                TicketId = request.TicketId,
                Status = request.Status,
                Category = request.Category
            };

            Result<HelpRequestDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.HelpRequests)
        .Produces<HelpRequestDto>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}
