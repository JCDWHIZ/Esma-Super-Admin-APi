using System;
using SharedKernel.Models;

namespace Domain.HelpRequests;

public class HelpRequestMessages : BaseAuditableEntity
{
    public string? Title { get; set; }
    public IList<string> Attachments { get; set; } = new List<string>();

    public int HelpRequestId { get; set; }
    public HelpRequests HelpRequest { get; set; } = default!;

}
