using System;
using Domain.HelpRequests;

namespace Application.HelpRequest.GetHelpRequestById;

public sealed class GetHelpRequestByIdQueryHandler(IApplicationDbContext _context) : IQueryHandler<GetHelpRequestByIdQuery, HelpRequestDto>
{
    public async Task<Result<HelpRequestDto>> Handle(GetHelpRequestByIdQuery query, CancellationToken cancellationToken)
    {
        HelpRequestDto? entity = await _context.HelpRequests
            .Where(hr => hr.PublicId == query.PublicId)
            .Select(hr => new HelpRequestDto
            {
                Id = hr.Id,
                TicketId = hr.TicketId,
                Status = hr.Status,
                PublicId = hr.PublicId,
                Messages = (hr.Messages ?? new List<HelpRequestMessages>()).Select(m => new HelpRequestMessageDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Attachments = m.Attachments,
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            return Result.Failure<HelpRequestDto>(HelpRequestErrors.NotFound(query.PublicId));
        }

        return Result.Success(entity);
    }
}
