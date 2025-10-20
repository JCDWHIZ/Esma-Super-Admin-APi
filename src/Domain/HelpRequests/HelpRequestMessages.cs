using SharedKernel.Models;

namespace Domain.HelpRequests;

public sealed class HelpRequestMessages : BaseAuditableEntity
{
    public string? Title { get; set; }
    public IList<string> Attachments { get; set; } = new List<string>();
    public string UserName { get; set; }
    public string UserProfilePic { get; set; }
    public int HelpRequestId { get; set; }
    public HelpRequests HelpRequest { get; set; } = default!;

}
