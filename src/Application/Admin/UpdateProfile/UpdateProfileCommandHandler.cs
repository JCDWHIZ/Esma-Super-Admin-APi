namespace Application.Admin.UpdateProfile;
public class UpdateProfileCommandHandler(IApplicationDbContext _context) : ICommandHandler<UpdateProfileCommand, string>
{
    public async Task<Result<string>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.PublicId == command.PublicId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<string>(UserErrors.NotFound(command.PublicId));
        }

        user.ProfilePic = command.ProfilePic;
        user.PhoneNumber = command.PhoneNumber;
        user.Email = command.Email;
        user.Username = command.Username;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("Profile updated successfully");
    }
}
