

namespace Application.HelpRequest;

public class HelpRequestDto
{
    public int Id { get; set; }
    public string? TicketId { get; set; }
    public HelpStatus Status { get; set; } = HelpStatus.OPEN_REQUEST;
    public HelpCategory? Category { get; set; }
    public Guid PublicId { get; set; }
    public ICollection<HelpRequestMessageDto> Messages { get; set; } = new List<HelpRequestMessageDto>();
}
