using Application.BackgroundJobs;
using Hangfire;

namespace Application.Admin.DeleteAdmin;
public sealed class DeleteAdminCommandHandler(IApplicationDbContext _context)
    : ICommandHandler<DeleteAdminCommand, string>
{
    public async Task<Result<string>> Handle(DeleteAdminCommand command, CancellationToken cancellationToken)
    {
        User? user = await _context.Users
            .FirstOrDefaultAsync(u => u.PublicId == command.PublicId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<string>(UserErrors.NotFound(command.PublicId));
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);

        BackgroundJob.Enqueue<IKeycloakOrganizationService>(
            service => service.DeleteAdmin(user.Id, cancellationToken));

        return $"User {user.Email} deleted successfully.";
    }
}
