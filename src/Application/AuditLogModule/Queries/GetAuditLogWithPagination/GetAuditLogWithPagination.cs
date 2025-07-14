using System;
using admin_service.Application.Common.Interfaces;
using admin_service.Application.Common.Mappings;
using admin_service.Application.Common.Models;

namespace admin_service.Application.AuditLogModule.Queries.GetAuditLogWithPagination;


public record GetAuditLogWithPaginationQuery : IRequest<PaginatedList<AuditLogDto>>
{
    public string? UserName { get; set; }
    public string? Action { get; set; }
    public string? Role { get; set; }
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
}

public class GetAuditLogWithPagination : IRequestHandler<GetAuditLogWithPaginationQuery, PaginatedList<AuditLogDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAuditLogWithPagination(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Task<PaginatedList<AuditLogDto>> Handle(GetAuditLogWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Auditlog.AsQueryable();

        if (!string.IsNullOrEmpty(request.UserName))
        {
           query = query.Where(a => _context.Users
            .Where(u => u.Username.Contains(request.UserName))
            .Select(u => u.Id.ToString())
            .Contains(a.CreatedBy));
        }
        if (!string.IsNullOrEmpty(request.Role))
        {
            query = query.Where(a => a.Role.Contains(request.Role));
        }

        if (!string.IsNullOrEmpty(request.Action))
        {
            query = query.Where(a => a.Action.Contains(request.Action));
        }

        return query
            .ProjectTo<AuditLogDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber ?? 1, request.PageSize ?? 10);
    }
}