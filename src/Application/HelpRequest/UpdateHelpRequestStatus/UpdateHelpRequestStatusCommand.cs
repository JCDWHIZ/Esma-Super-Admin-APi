using System;

namespace Application.HelpRequest.UpdateHelpRequestStatus;

public sealed record UpdateHelpRequestStatusCommand : ICommand<HelpRequestDto>
{
    public Guid PublicId { get; init; }
    public HelpStatus Status { get; init; }
}