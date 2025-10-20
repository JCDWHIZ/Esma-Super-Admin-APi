namespace Application.HelpRequest.GetHelpRequestWithPagination;

public record GetHelpRequestWithPaginationQuery : IQuery<PaginatedList<HelpRequestDto>>
{
    public string? TicketId { get; set; }
    public HelpStatus? Status { get; set; }
    public HelpCategory? Category { get; set; }
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
}
