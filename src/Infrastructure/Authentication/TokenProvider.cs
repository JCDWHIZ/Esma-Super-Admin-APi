using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Abstractions.Authentication;
using Domain.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Infrastructure.Authentication;

internal sealed class TokenProvider(IConfiguration configuration) : ITokenProvider
{
    private readonly SymmetricSecurityKey _securityKey = new(
        Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!));
    private readonly string _issuer = configuration["Jwt:Issuer"]!;
    private readonly string _audience = configuration["Jwt:Audience"]!;

    public string Create(User user)
    {
        string secretKey = configuration["Jwt:Secret"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.PublicId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("Jwt:ExpirationInMinutes")),
            SigningCredentials = credentials,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"]
        };

        var handler = new JsonWebTokenHandler();

        string token = handler.CreateToken(tokenDescriptor);

        return token;
    }


    /// <summary>
    /// Creates a JWT valid for 1 hour that contains PublicId, Email, RoleId, and RoleName.
    /// </summary>
    public string CreateOnboardingToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        // ArgumentNullException.ThrowIfNull(user.Role);
        // if (user.Role is null)
        // {
        //     throw new InvalidOperationException("User.Role cannot be null when creating onboarding token.");
        // }

        // Build claims for onboarding
        Claim[] claims = new[]
            {
                new Claim("publicId", user.PublicId.ToString()),
                new Claim("email", user.Email),
                // new Claim("roleId", user.Role.PublicId.ToString()),
                // new Claim("roleName", user.Role.Name),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        var creds = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(48),
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            SigningCredentials = creds,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        string tokenString = tokenHandler.WriteToken(token);

        Console.WriteLine($"Created token: {tokenString}");
        Console.WriteLine($"Issued At: {DateTime.UtcNow}, Expires At: {tokenDescriptor.Expires}");
        Console.WriteLine($"Secret: {Convert.ToBase64String(_securityKey.Key)}, Issuer: {_issuer}, Audience: {_audience}");

        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateOnboardingToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _securityKey,
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out _);

            // Fix here: match actual claim type names
            bool hasAllClaims =
                principal.HasClaim(c => c.Type == "publicId") &&
                principal.HasClaim(c => c.Type == ClaimTypes.Email);

            Console.WriteLine($"Validated claims: {string.Join(", ", principal.Claims.Select(c => $"{c.Type}: {c.Value}"))}");

            return hasAllClaims ? principal : null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            return null;
        }
    }

}
