using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.BackgroundJobs;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.EditAdmin;
public sealed class EditAdminCommandHandler(IApplicationDbContext _context) : ICommandHandler<EditAdminCommand, UserDto>
{
    public async Task<Result<UserDto>> Handle(EditAdminCommand command, CancellationToken cancellationToken)
    {
        User? user = await _context.Users
          .FirstOrDefaultAsync(u => u.PublicId == command.PublicId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<UserDto>(UserErrors.NotFound(command.PublicId));
        }

        user.FirstName = command.FirstName ?? user.FirstName;
        user.LastName = command.LastName ?? user.LastName;
        user.Email = command.Email ?? user.Email;
        user.PhoneNumber = command.PhoneNumber ?? user.PhoneNumber;
        user.ProfilePic = command.ProfilePic ?? user.ProfilePic;
        user.Role = command.Role;

        await _context.SaveChangesAsync(cancellationToken);
        BackgroundJob.Enqueue<IKeycloakOrganizationService>(
        service => service.EditAdmin(user.Id, cancellationToken));

        return new UserDto
        {
            PublicId = user.PublicId,
            Email = user.Email,
            Role = user.Role,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            ProfilePic = user.ProfilePic,
            CreatedAt = user.Created,
            CreatedBy = user.CreatedBy
        };
    }
}
