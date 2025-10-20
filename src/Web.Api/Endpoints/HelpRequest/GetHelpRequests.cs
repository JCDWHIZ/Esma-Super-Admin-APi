using Application;
using Application.HelpRequest;
using Application.HelpRequest.GetHelpRequestWithPagination;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.HelpRequests;

internal sealed class GetPaginated : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("help-requests", async (
            string? ticketId,
            HelpStatus? status,
            HelpCategory? category,
            int? pageNumber,
            int? pageSize,
            IQueryHandler<GetHelpRequestWithPaginationQuery, PaginatedList<HelpRequestDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetHelpRequestWithPaginationQuery
            {
                TicketId = ticketId,
                Status = status,
                Category = category,
                PageNumber = pageNumber ?? 1,
                PageSize = pageSize ?? 10
            };

            Result<PaginatedList<HelpRequestDto>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.HelpRequests)
        .Produces<PaginatedList<HelpRequestDto>>()
        .RequireAuthorization(new RequirePermissionAttribute("help_request_view"));
    }
}
