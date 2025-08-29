using System;
using Application.Abstractions.Models;

namespace Application.AuditLogModule.GetAuditLogs;

public sealed class GetAuditLogQueryHandler(IApplicationDbContext _context) : IQueryHandler<GetAuditLogsQuery, PaginatedList<AuditLogDto>>
{
    public async Task<Result<PaginatedList<AuditLogDto>>> Handle(GetAuditLogsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Domain.AuditLogs.AuditLog> Auditquery = _context.Auditlog.AsQueryable();

        // Combined search term for username or action
        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            Auditquery = Auditquery.Where(a =>
                _context.Users.Any(u => u.Username.Contains(query.SearchTerm)) ||
                a.Action.Contains(query.SearchTerm));
        }

        if (query.TimeFilter.HasValue)
        {
            DateTime currentDate = DateTime.UtcNow;
            switch (query.TimeFilter)
            {
                case TimeFilter.LAST_30_DAYS:
                    Auditquery = Auditquery.Where(a => a.Created >= currentDate.AddDays(-30));
                    break;
                case TimeFilter.LAST_60_DAYS:
                    Auditquery = Auditquery.Where(a => a.Created >= currentDate.AddDays(-60));
                    break;
                case TimeFilter.LAST_90_DAYS:
                    Auditquery = Auditquery.Where(a => a.Created >= currentDate.AddDays(-90));
                    break;
                case TimeFilter.OVER_90_DAYS:
                    Auditquery = Auditquery.Where(a => a.Created < currentDate.AddDays(-90));
                    break;
            }
        }
        if (!string.IsNullOrEmpty(query.Role))
        {
            Auditquery = Auditquery.Where(a => a.Role.Contains(query.Role));
        }

        PaginatedList<AuditLogDto> pagedAuditLogs = await PaginatedList<AuditLogDto>.CreateAsync(
            Auditquery.Select(a => new AuditLogDto
            {
                Role = a.Role,
                Action = a.Action,
                CreatedAt = a.Created,
                CreatedBy = a.CreatedBy,
                PublicId = a.PublicId
            }),
            query.PageNumber ?? 1,
            query.PageSize ?? 10
        );

        return Result.Success(pagedAuditLogs);
    }
}
