using System;
using admin_service.Application.Common.Interfaces;
using admin_service.Domain.Entities;

namespace admin_service.Application.BlogModule.Queries.GetBlogById;


public record GetBlogByIdQuery(string PublicId) : IRequest<BlogItemDto>;
public class GetBlogByIdQueryHandler : IRequestHandler<GetBlogByIdQuery, BlogItemDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetBlogByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BlogItemDto> Handle(GetBlogByIdQuery request,
        CancellationToken cancellationToken)
    {
        Blog? entity = await _context.Blog
            .FirstOrDefaultAsync(x => x.PublicId == request.PublicId, cancellationToken);
        Guard.Against.NotFound(request.PublicId, entity);
        return _mapper.Map<BlogItemDto>(entity);
    }
}
