using System;

namespace admin_service.Application.Common.Interfaces;

 public interface IAuthenticationService
    {
        Task<AuthResult> ValidateCredentialsAsync(string username, string password);
        Task<AuthResult> RefreshTokenAsync(string refreshToken);
        Task RevokeTokenAsync(string refreshToken);
    }

    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        public string? Error { get; set; }
    }