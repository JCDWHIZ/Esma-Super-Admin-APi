using Application.Auth.Login;

namespace Application.Auth.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<LoginCommandResponseDto>;
