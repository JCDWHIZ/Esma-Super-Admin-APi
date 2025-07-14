using System;
using admin_service.Application.Common.Interfaces;
using admin_service.Application.Common.Mappings;
using admin_service.Application.Common.Models;
using admin_service.Domain.Entities;
using admin_service.Domain.Enums;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace admin_service.Application.HelpRequest.Queries.GetHelpRequestWithPagination;

public record GetHelpRequestWithPaginationQuery : IRequest<PaginatedList<HelpRequestItemDto>>
{
    public string? TicketId { get; set; }
    public HelpStatus? Status { get; set; }
    public HelpCategory? Category { get; set; }
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
}

public class GetHelpRequestWithPaginationQueryHandler : IRequestHandler<GetHelpRequestWithPaginationQuery, PaginatedList<HelpRequestItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetHelpRequestWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<HelpRequestItemDto>> Handle(GetHelpRequestWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.HelpRequests.AsQueryable();

        if (!string.IsNullOrEmpty(request.TicketId))
        {
            query = query.Where(s => s.TicketId != null && s.TicketId.Contains(request.TicketId));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(s => s.Status == request.Status);
        }

        if (request.Category.HasValue)
        {
            query = query.Where(s => s.Category == request.Category);
        }

        return await query
            .ProjectTo<HelpRequestItemDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber ?? 1, request.PageSize ?? 10);
    }
}