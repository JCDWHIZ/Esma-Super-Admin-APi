using Application.Interfaces;

namespace Application.Auth;
public sealed record GetUserSessionQuery : IQuery<GetSessionsCommandResponseDto>;


public sealed class GetSessionsCommandResponseDto
{
    public List<KeycloakSessionDto> Sessions { get; init; } = new();
    public int TotalCount { get; init; }
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }

    public GetSessionsCommandResponseDto(
        List<KeycloakSessionDto> sessions,
        bool success = true,
        string? errorMessage = null)
    {
        Sessions = sessions ?? new List<KeycloakSessionDto>();
        TotalCount = Sessions.Count;
        Success = success;
        ErrorMessage = errorMessage;
    }
}
