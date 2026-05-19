using System.Security.Claims;
using Application.Abstractions.Authentication;
using Application.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Application.Auth.SetPassword;

public sealed class SetPasswordCommandHandler : ICommandHandler<SetPasswordCommand, string>
{
    private readonly ILogger<SetPasswordCommandHandler> _logger;
    private readonly ITokenProvider _tokenProvider;
    private readonly IApplicationDbContext _context;

    public SetPasswordCommandHandler(ILogger<SetPasswordCommandHandler> logger, ITokenProvider tokenProvider, IApplicationDbContext context)
    {
        _logger = logger;
        _tokenProvider = tokenProvider;
        _context = context;
    }

    public async Task<Result<string>> Handle(SetPasswordCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("User attempting to change password");

            ClaimsPrincipal? principal = _tokenProvider.ValidateOnboardingToken(command.AccessToken);
            if (principal == null)
            {
                _logger.LogWarning("Invalid or expired access token");
                return Result.Failure<string>(UserErrors.TokenError());
            }

            string? userPublicId = principal.Claims.FirstOrDefault(c => c.Type == "publicId")?.Value;
            if (string.IsNullOrEmpty(userPublicId))
            {
                _logger.LogWarning("User public ID not found in token");
                return Result.Failure<string>(UserErrors.NotFoundInToken);
            }


            User? user = await _context.Users
                .FirstOrDefaultAsync(u => u.PublicId.ToString() == userPublicId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User not found for public ID: {UserPublicId}", userPublicId);
                return Result.Failure<string>(UserErrors.NotFound(userPublicId));
            }

            if (user.KeycloakUserId == Guid.Empty)
            {
                _logger.LogWarning("KeycloakUserId not set for user: {UserPublicId}", userPublicId);
                return Result.Failure<string>(UserErrors.NotFound(userPublicId));
            }
            BackgroundJob.Enqueue<IKeycloakService>(
            service => service.ResetPasswordAsync(user.KeycloakUserId, command.NewPassword));
            return Result.Success("Password has been reset");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return Result.Failure<string>(UserErrors.ErrorOccured());
        }
    }
}
