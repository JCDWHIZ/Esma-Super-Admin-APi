using System;
using Application.Abstractions.Models;

namespace Application.AuditLogModule.GetAuditLogs;

public sealed class GetAuditLogQueryHandler(IApplicationDbContext _context) : IQueryHandler<GetAuditLogsQuery, PaginatedList<AuditLogDto>>
{
    public async Task<Result<PaginatedList<AuditLogDto>>> Handle(GetAuditLogsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Domain.AuditLogs.AuditLog> Auditquery = _context.Auditlog.AsQueryable();

        if (!string.IsNullOrEmpty(query.UserName))
        {
            Auditquery = Auditquery.Where(a => _context.Users.Any(u => u.Username.Contains(query.UserName)));
        }
        if (!string.IsNullOrEmpty(query.Role))
        {
            Auditquery = Auditquery.Where(a => a.Role.Contains(query.Role));
        }

        if (!string.IsNullOrEmpty(query.Action))
        {
            Auditquery = Auditquery.Where(a => a.Action.Contains(query.Action));
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
