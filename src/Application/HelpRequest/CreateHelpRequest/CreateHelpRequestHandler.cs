using System;
using admin_service.Application.Common.Interfaces;
using admin_service.Application.HelpRequest.Queries;
using admin_service.Domain.Entities;
using admin_service.Domain.Enums;

namespace admin_service.Application.HelpRequest.Commands.CreateHelpRequest;


public record IntiateHelpRequestCommand: IRequest<HelpRequestItemDto>
{
    public string? TicketId { get; set; } = Math.Abs(Guid.NewGuid().GetHashCode()).ToString();
    public HelpStatus Status { get; set; } = HelpStatus.OPEN_REQUEST;
    public HelpCategory Category { get; set; }
}


public class CreateHelpRequestHandler : IRequestHandler<IntiateHelpRequestCommand, HelpRequestItemDto>
{
    private readonly IApplicationDbContext _dbContext;

    private readonly IMapper _mapper;
    public CreateHelpRequestHandler(IApplicationDbContext dbContext,  IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    async Task<HelpRequestItemDto> IRequestHandler<IntiateHelpRequestCommand, HelpRequestItemDto>.Handle(IntiateHelpRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = new HelpRequests
        {
            TicketId = request.TicketId,
            Category = request.Category,
            Status = request.Status,
        };
    _dbContext.HelpRequests.Add(entity);
    await _dbContext.SaveChangesAsync(cancellationToken);
    return _mapper.Map<HelpRequestItemDto>(entity);
    }
}
