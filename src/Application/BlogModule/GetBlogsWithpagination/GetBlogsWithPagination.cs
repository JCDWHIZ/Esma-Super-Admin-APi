using System;
using admin_service.Application.Common.Interfaces;
using admin_service.Application.Common.Models;
using admin_service.Domain.Enums;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using admin_service.Application.Common.Mappings;

namespace admin_service.Application.BlogModule.Queries.GetBlogsWithpagination;



public record GetBlogWithPaginationQuery : IRequest<PaginatedList<BlogItemDto>>
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? BackdropUrl {get; set;}
    public BlogStatus? Status { get; set; }
    public DateTime? PublishDate { get; set; }
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
}


public class GetBlogsWithPaginationQueryHandler : IRequestHandler<GetBlogWithPaginationQuery, PaginatedList<BlogItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetBlogsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public Task<PaginatedList<BlogItemDto>> Handle(GetBlogWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Blog.AsQueryable();
         if (!string.IsNullOrEmpty(request.Title))
        {
            query = query.Where(b => b.Title.Contains(request.Title));
        }

        if (!string.IsNullOrEmpty(request.Content))
        {
            query = query.Where(b => b.Content.Contains(request.Content));
        }

        if (!string.IsNullOrEmpty(request.BackdropUrl))
        {
            query = query.Where(b => b.BackdropUrl.Contains(request.BackdropUrl));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(b => b.Status == request.Status.Value);
        }

        if (request.PublishDate.HasValue)
        {
            query = query.Where(b => b.PublishDate == request.PublishDate.Value);
        }
        return query
            .ProjectTo<BlogItemDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber ?? 1, request.PageSize ?? 10);
    }
}
