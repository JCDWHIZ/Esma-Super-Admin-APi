using System;
using SharedKernel.Enums;
using SharedKernel.Models;

namespace Domain.HelpRequests;

public sealed class HelpRequests : BaseAuditableEntity
{
    public string? TicketId { get; set; }
    public HelpStatus Status { get; set; } = HelpStatus.OPEN_REQUEST;
    public HelpCategory? Category { get; set; }

    public ICollection<HelpRequestMessages>? Messages { get; set; } = new List<HelpRequestMessages>();

}
