using System;
using Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Application.Auth.Login;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginCommandResponseDto>
{
    private readonly KeycloakService _keycloakService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(KeycloakService keycloakService, ILogger<LoginCommandHandler> logger)
    {
        _keycloakService = keycloakService;
        _logger = logger;
    }

    public async Task<Result<LoginCommandResponseDto>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Attempting login for user: {Email}", command.Email);
            LoginCommandResponseDto loginResponse = await _keycloakService.LoginAsync(command.Email, command.Password);

            _logger.LogInformation("Login successful for user: {Email}", command.Email);

            return Result.Success(loginResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Login failed for user {Email}", command.Email);
            return (Result<LoginCommandResponseDto>)Result.Failure(UserErrors.ErrorOccured());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error for user {Email}", command.Email);
            return (Result<LoginCommandResponseDto>)Result.Failure(UserErrors.ErrorOccured());
        }
    }
}
