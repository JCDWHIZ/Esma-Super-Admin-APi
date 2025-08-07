using System;
using Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Application.Auth.ForgotPassword;

public sealed class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, ForgotPasswordResponseDto>
{
    private readonly KeycloakService _keycloakService;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(KeycloakService keycloakService, ILogger<ForgotPasswordCommandHandler> logger)
    {
        _keycloakService = keycloakService;
        _logger = logger;
    }

    public async Task<Result<ForgotPasswordResponseDto>> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing password reset request for email: {Email}", command.Email);

            ForgotPasswordResponseDto result = await _keycloakService.SendPasswordResetEmailAsync(command.Email, cancellationToken);

            if (result.Success)
            {
                _logger.LogInformation("Password reset email sent successfully for: {Email}", command.Email);
            }
            else
            {
                _logger.LogWarning("Failed to send password reset email for: {Email}. Reason: {Message}", command.Email, result.Message);
            }

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing password reset for email: {Email}", command.Email);
            return Result.Failure<ForgotPasswordResponseDto>(UserErrors.ErrorOccured());
        }
    }
}