using System;
using Application.Interfaces;
using admin_service.Application.HelpRequest.Queries;
using admin_service.Domain.Entities;
using admin_service.Domain.Enums;

namespace admin_service.Application.HelpRequest.Commands.UpdateHelpRequestStatus;

public record UpdateHelpRequestStatusCommand : ICommand<HelpRequestItemDto>
{
    public string PublicId { get; init; } = string.Empty;
    public HelpStatus Status { get; init; }
}

public class UpdateHelpRequestStatus(IApplicationDbContext context, IMapper mapper)
    : ICommandHandler<UpdateHelpRequestStatusCommand, HelpRequestItemDto>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<HelpRequestItemDto> Handle(
        UpdateHelpRequestStatusCommand request,
        CancellationToken cancellationToken
    )
    {
        var entity = await _context.HelpRequests.FirstOrDefaultAsync(x => x.PublicId == request.PublicId);
        Guard.Against.NotFound(request.PublicId, entity); // Ensure HelpRequestItemDto has an Id property
        entity.Status = request.Status;

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<HelpRequestItemDto>(entity);
    }
}
