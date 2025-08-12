using System;
using Application.Auth.Login;
using Application.Interfaces;
using Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Application.Auth.RefreshToken;

public sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, LoginCommandResponseDto>
{
    private readonly KeycloakService _keycloakService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(KeycloakService keycloakService, ILogger<RefreshTokenCommandHandler> logger)
    {
        _keycloakService = keycloakService;
        _logger = logger;
    }

    public async Task<Result<LoginCommandResponseDto>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Attempting to refresh token");

            LoginCommandResponseDto refreshedToken = await _keycloakService.RefreshTokenAsync(command.RefreshToken);

            _logger.LogInformation("Token refreshed successfully");

            return Result.Success(refreshedToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return Result.Failure<LoginCommandResponseDto>(UserErrors.ErrorOccured());
        }
    }
}
