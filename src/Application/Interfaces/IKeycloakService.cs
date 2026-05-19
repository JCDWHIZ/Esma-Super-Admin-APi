using Application.Abstractions.Models;
using Application.Auth.ForgotPassword;
using Application.Auth.Login;

namespace Application.Interfaces;

public interface IKeycloakService
{
    Task<string> GetAdminAccessTokenAsync();
    Task<string> CreateOrganizationAsync(string schoolName);
    Task<string> CreateUserAsync(InviteUserRequestDto request);
    Task UpdateUserAsync(string keycloakUserId, UpdateUserRequestDto request);
    Task DeleteUserAsync(string keycloakUserId);
    Task AddUserToOrganizationAsync(string userId, string? organizationId = null);
    Task<LoginCommandResponseDto> LoginAsync(string email, string password);
    Task<LoginCommandResponseDto> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync(string refreshToken);
    Task<ForgotPasswordResponseDto> SendPasswordResetEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> ValidateTokenAsync(string accessToken);
    Task<KeycloakUserDto?> GetUserByEmailAsync(string email);
    Task<bool> SendUserSetupEmailAsync(string keycloakUserId);
    Task<SetPasswordResponseDto> ChangeUserPasswordAsync(string currentPassword, string newPassword, string userAccessToken);
    Task ResetPasswordAsync(Guid keycloakUserId, string password);
}

