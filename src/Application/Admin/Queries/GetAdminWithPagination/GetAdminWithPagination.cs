using System;
using admin_service.Application.Common.Interfaces;
using admin_service.Application.Common.Mappings;
using admin_service.Application.Common.Models;
using admin_service.Domain.Enums;

namespace admin_service.Application.Admin.Queries;


public record GetAdminWithPaginationQuery : IRequest<PaginatedList<UserDto>>
{
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
    public string? Username { get; set; }
    public Roles? Role { get; set; }
}

public class GetAdminWithPaginationQueryHandler : IRequestHandler<GetAdminWithPaginationQuery, PaginatedList<UserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAdminWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Task<PaginatedList<UserDto>> Handle(GetAdminWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(request.Username))
        {
            query = query.Where(x => x.Username.Contains(request.Username));
        }
        if (request.Role.HasValue)
        {
            query = query.Where(x => x.Role == request.Role.Value);
        }
        return query.ProjectTo<UserDto>(_mapper.ConfigurationProvider)
        .PaginatedListAsync(request.PageNumber ?? 1, request.PageSize ?? 10);
    }
}

