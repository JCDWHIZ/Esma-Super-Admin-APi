using Application.Abstractions.Authentication;
using Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Application.Auth.ChangePassword;
public class ChangePasswordCommandHandler(KeycloakService _keycloakService, ILogger<ChangePasswordCommandHandler> _logger, IUserContext _userContext) : ICommandHandler<ChangePasswordCommand, string>
{
    public async Task<Result<string>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Attempting password change for authenticated user");
            var keycloakUserId = Guid.Parse(_userContext.KeycloakId ?? "");
            await _keycloakService.ResetPasswordAsync(keycloakUserId, command.NewPassword);

            //if (result)
            //{
            //    _logger.LogInformation("Password change successful for authenticated user");
            //    return Result.Success("Password change successful");
            //}
            //return Result.Failure<string>(UserErrors.ErrorOccured());
            return Result.Success("Password change successful");
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Password change failed due to unauthorized access");
            return Result.Failure<string>(UserErrors.InvalidCurrentPassword());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during password change");
            return Result.Failure<string>(UserErrors.ErrorOccured());
        }
    }
}
