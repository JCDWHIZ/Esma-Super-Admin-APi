using Application.BackgroundJobs;
using Hangfire;

namespace Application.Admin.EditAdmin;
public sealed class EditAdminCommandHandler(IApplicationDbContext _context) : ICommandHandler<EditAdminCommand, UserDto>
{
    public async Task<Result<UserDto>> Handle(EditAdminCommand command, CancellationToken cancellationToken)
    {
        User? user = await _context.Users
          .Include(u => u.Role) // Include Role to access its properties
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

        Domain.Roles.Role? role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == command.RoleName, cancellationToken);
        if (role != null)
        {
            user.RoleId = role.Id; // Update RoleId if a new role is provided
        }

        await _context.SaveChangesAsync(cancellationToken);
        BackgroundJob.Enqueue<IKeycloakOrganizationService>(
            service => service.EditAdmin(user.Id, cancellationToken));

        return new UserDto
        {
            PublicId = user.PublicId,
            Email = user.Email,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            ProfilePic = user.ProfilePic,
            RoleName = user.Role?.Name, // Use Role.Name from navigation property
            CreatedAt = user.Created,
            CreatedBy = user.CreatedBy
        };
    }
}
