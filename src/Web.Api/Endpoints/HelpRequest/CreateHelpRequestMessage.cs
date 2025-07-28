using Application.HelpRequest.CreateHelpReqestMessage;
using Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Infrastructure;
using Web.Api.Extensions;
using Application.HelpRequest;

namespace Web.Api.Endpoints.HelpRequest;

internal sealed class CreateHelpRequestMessage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("help-requests/{helpRequestId:int}/messages", async (
            int helpRequestId,
            CreateHelpRequestMessageRequest request,
            ICommandHandler<CreateHelpRequestMessageCommand, HelpRequestMessageDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateHelpRequestMessageCommand
            {
                HelpRequestId = helpRequestId,
                Title = request.Title,
                Attachments = request.Attachments ?? new List<string>()
            };

            Result<HelpRequestMessageDto> result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.HelpRequests)
        .Produces<HelpRequestMessageDto>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}

internal sealed class CreateHelpRequestMessageRequest
{
    public string Title { get; set; } = string.Empty;
    public ICollection<string>? Attachments { get; set; }
}
