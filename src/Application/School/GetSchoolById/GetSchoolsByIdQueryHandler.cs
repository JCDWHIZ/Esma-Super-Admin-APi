using System;
using admin_service.Application.Common.Interfaces;
using admin_service.Domain.Entities;

namespace admin_service.Application.School.Queries.GetSchoolsWithById;


public record GetSchoolByIdQuery(string PublicId) : IRequest<SchoolItemDto>;
public class GetSchoolsWithByIdQueryHandler : IRequestHandler<GetSchoolByIdQuery, SchoolItemDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetSchoolsWithByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<SchoolItemDto> Handle(GetSchoolByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await _context.Schools
        .Include(s => s.Subscriptions)
            .FirstOrDefaultAsync(x => x.PublicId == request.PublicId, cancellationToken);
        Guard.Against.NotFound(request.PublicId, entity);
        return _mapper.Map<SchoolItemDto>(entity);
    }

}
