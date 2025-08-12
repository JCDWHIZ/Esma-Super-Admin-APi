using Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Authentication;

internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserPublicId =>
        _httpContextAccessor
            .HttpContext?
            .User
            .GetUserPublicId();

    public string? UserRole => 
        _httpContextAccessor
           .HttpContext?
           .User
           .GetUserRole();
}
