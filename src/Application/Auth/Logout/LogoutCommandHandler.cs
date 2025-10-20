using Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Application.Auth.Logout;

public sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand, bool>
{
    private readonly KeycloakService _keycloakService;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(KeycloakService keycloakService, ILogger<LogoutCommandHandler> logger)
    {
        _keycloakService = keycloakService;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Attempting logout");

            bool success = await _keycloakService.LogoutAsync(command.RefreshToken);

            if (success)
            {
                _logger.LogInformation("Logout successful");
                return Result.Success(true);
            }
            else
            {
                _logger.LogWarning("Logout failed");
                return Result.Failure<bool>(UserErrors.ErrorOccured());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return Result.Failure<bool>(UserErrors.ErrorOccured());
        }
    }
}