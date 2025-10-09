using System;
using Application;
using Application.AuditLogModule;
using Application.AuditLogModule.GetAuditLogs;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.AuditLogs;

internal sealed class GetAuditLogs : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("audit-logs", async (
            string? searchTerm,
            string? action,
            string? role,
            TimeFilter ? timeFilter,
            int ? pageNumber,
            int? pageSize,
            IQueryHandler<GetAuditLogsQuery, PaginatedList<AuditLogDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAuditLogsQuery
            {
                SearchTerm = searchTerm,
                Action = action,
                Role = role,
                TimeFilter = timeFilter,
                PageNumber = pageNumber ?? 1,
                PageSize = pageSize ?? 10
            };

            Result<PaginatedList<AuditLogDto>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.AuditLogs)
        .Produces<PaginatedList<AuditLogDto>>()
        .RequireAuthorization(new RequirePermissionAttribute("audit_log_view"));
    }
}
