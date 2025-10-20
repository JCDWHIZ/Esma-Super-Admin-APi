namespace Application.HelpRequest.GetHelpRequestById;

public sealed record GetHelpRequestByIdQuery(Guid PublicId) : IQuery<HelpRequestDto>;