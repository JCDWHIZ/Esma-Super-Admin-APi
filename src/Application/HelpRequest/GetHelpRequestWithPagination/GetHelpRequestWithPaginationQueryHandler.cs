using System;
using Application.Interfaces;
using Application.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.HelpRequest.GetHelpRequestWithPagination;

public sealed class GetHelpRequestWithPaginationQueryHandler(IApplicationDbContext _context) : IQueryHandler<GetHelpRequestWithPaginationQuery, PaginatedList<HelpRequestDto>>
{
    public async Task<Result<PaginatedList<HelpRequestDto>>> Handle(GetHelpRequestWithPaginationQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Domain.HelpRequests.HelpRequests> helpRequestQuery = _context.HelpRequests.AsQueryable();

        if (!string.IsNullOrEmpty(query.TicketId))
        {
            helpRequestQuery = helpRequestQuery.Where(s => s.TicketId != null && s.TicketId.Contains(query.TicketId));
        }

        if (query.Status.HasValue)
        {
            helpRequestQuery = helpRequestQuery.Where(s => s.Status == query.Status);
        }

        if (query.Category.HasValue)
        {
            helpRequestQuery = helpRequestQuery.Where(s => s.Category == query.Category);
        }

        PaginatedList<HelpRequestDto> paginatedList = await PaginatedList<HelpRequestDto>.CreateAsync(
             helpRequestQuery.Select(s => new HelpRequestDto
             {
                 Id = s.Id,
                 TicketId = s.TicketId,
                 Status = s.Status,
                 PublicId = s.PublicId,
                 Category = s.Category,
                 Messages = new List<HelpRequestMessageDto>()
             }),
             query.PageNumber ?? 1,
             query.PageSize ?? 10
         );

        return Result.Success(paginatedList);
    }
}