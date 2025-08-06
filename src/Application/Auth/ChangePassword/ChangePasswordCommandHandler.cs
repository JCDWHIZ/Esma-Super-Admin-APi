using System;
using Application.Interfaces;
using Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Application.Auth.ChangePassword;

public sealed class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, ChangePasswordResponseDto>
{
    private readonly KeycloakService _keycloakService;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(KeycloakService keycloakService, ILogger<ChangePasswordCommandHandler> logger)
    {
        _keycloakService = keycloakService;
        _logger = logger;
    }

    public async Task<Result<ChangePasswordResponseDto>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("User attempting to change password");

            if (command.CurrentPassword == command.NewPassword)
            {
                return (Result<ChangePasswordResponseDto>)Result.Failure(UserErrors.PasswordIncorrect());
            }

            ChangePasswordResponseDto result = await _keycloakService.ChangeUserPasswordAsync(command.CurrentPassword, command.NewPassword, command.AccessToken);

            if (result.Success)
            {
                _logger.LogInformation("Password changed successfully");
            }
            else
            {
                _logger.LogWarning("Failed to change password. Reason: {Message}", result.Message);
            }

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return (Result<ChangePasswordResponseDto>)Result.Failure(UserErrors.ErrorOccured());
        }
    }
}