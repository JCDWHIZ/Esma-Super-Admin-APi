using System;

namespace Application.Auth.Login;

public class LoginCommandResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string IdToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public int RefreshExpiresIn { get; set; }
    public string TokenType { get; set; } = string.Empty;
    public string SessionState { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime RefreshExpiresAt { get; set; }
}
