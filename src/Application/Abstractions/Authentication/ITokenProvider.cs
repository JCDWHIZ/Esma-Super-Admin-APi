using System.Security.Claims;

namespace Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string Create(User user);
    string CreateOnboardingToken(User user);
    ClaimsPrincipal? ValidateOnboardingToken(string token);
}
