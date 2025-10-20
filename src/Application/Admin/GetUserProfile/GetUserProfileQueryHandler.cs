using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Authentication;

namespace Application.Admin.GetUserProfile;
public class GetUserProfileQueryHandler(IApplicationDbContext _context, IUserContext userContext) : IQueryHandler<GetUserProfileQuery, UserDto>
{
    public async Task<Result<UserDto>> Handle(GetUserProfileQuery query, CancellationToken cancellationToken)
    {
        UserDto? user = await _context.Users
            .Where(u => u.PublicId == userContext.UserPublicId)
            .Select(u => new UserDto
            {
                PublicId = u.PublicId,
                Username = u.Username,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                ProfilePic = u.ProfilePic
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null)
        {
            return Result.Failure<UserDto>(UserErrors.NotFound(userContext.UserPublicId ?? Guid.Empty));
        }
        return Result.Success(user);
    }
}
