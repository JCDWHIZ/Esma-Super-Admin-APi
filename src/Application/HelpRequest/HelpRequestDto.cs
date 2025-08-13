

namespace Application.HelpRequest;

public class HelpRequestDto
{
    public int Id { get; set; }
    public string? TicketId { get; set; }
    public HelpStatus Status { get; set; } = HelpStatus.OPEN_REQUEST;
    public HelpCategory? Category { get; set; }
    public Guid PublicId { get; set; }
    public string TenantHelpRequestId { get; set; }
    public string SchoolId { get; set; }
    public string UserProfilePic { get; set; }
    public string UserName { get; set; } = string.Empty;
    public ICollection<HelpRequestMessageDto> Messages { get; set; } = new List<HelpRequestMessageDto>();
}
