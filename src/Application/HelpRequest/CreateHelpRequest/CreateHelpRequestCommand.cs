using System;

namespace Application.HelpRequest.CreateHelpRequest;

public sealed record CreateHelpRequestCommand : ICommand<HelpRequestDto>
{
    public string? TicketId { get; set; } = Math.Abs(Guid.NewGuid().GetHashCode()).ToString(System.Globalization.CultureInfo.InvariantCulture);
    public HelpStatus Status { get; set; } = HelpStatus.OPEN_REQUEST;
    public HelpCategory Category { get; set; }
}