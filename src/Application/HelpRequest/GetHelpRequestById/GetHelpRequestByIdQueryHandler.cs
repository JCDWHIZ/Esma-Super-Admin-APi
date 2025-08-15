using System;
using Domain.HelpRequests;

namespace Application.HelpRequest.GetHelpRequestById;

public sealed class GetHelpRequestByIdQueryHandler(IApplicationDbContext _context) : IQueryHandler<GetHelpRequestByIdQuery, HelpRequestDto>
{
    public async Task<Result<HelpRequestDto>> Handle(GetHelpRequestByIdQuery query, CancellationToken cancellationToken)
    {
        HelpRequestDto? entity = await _context.HelpRequests
            .Include(hr => hr.Messages) // Eagerly load Messages
            .Where(hr => hr.PublicId == query.PublicId)
            .Select(hr => new HelpRequestDto
            {
                Id = hr.Id,
                TicketId = hr.TicketId,
                Status = hr.Status,
                PublicId = hr.PublicId,
                Category = hr.Category,
                TenantHelpRequestId = hr.TenantHelpRequestId,
                SchoolId = hr.SchoolId,
                UserName = hr.UserName,
                Messages = hr.Messages == null
                    ? new List<HelpRequestMessageDto>()
                    : hr.Messages.Select(m => new HelpRequestMessageDto
                    {
                        Id = m.Id,
                        PublicId = m.PublicId,
                        Title = m.Title,
                        UserName = m.UserName,
                        UserProfilePic = m.UserProfilePic,
                        Attachments = m.Attachments ?? new List<string>()
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
