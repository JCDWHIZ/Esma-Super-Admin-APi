using System.Security.Claims;
using Domain.Users;

namespace Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string Create(User user);
    string CreateOnboardingToken(User user);
    ClaimsPrincipal? ValidateOnboardingToken(string token);
}
