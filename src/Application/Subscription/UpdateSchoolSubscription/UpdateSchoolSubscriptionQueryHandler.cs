using Domain.Schools;

namespace Application.Subscription.UpdateSchoolSubscription;

public sealed class UpdateSchoolSubscriptionHandler(IApplicationDbContext _context)
    : ICommandHandler<UpdateSchoolSubscriptionCommand, string>
{
    public async Task<Result<bool>> Handle(UpdateSchoolSubscriptionCommand command, CancellationToken cancellationToken)
    {
        Schools? school = await _context.Schools
            .Include(s => s.Subscriptions)
            .FirstOrDefaultAsync(s => s.PublicId == command.SchoolId, cancellationToken);

        if (school is null)
        {
            return Result.Failure<bool>(SchoolErrors.NotFound(command.SchoolId));
        }

        school.Subscriptions.SubscriptionType = command.SubscriptionType;
        school.Subscriptions.StartDate = command.StartDate;
        school.Subscriptions.EndDate = command.EndDate;
        school.Subscriptions.Amount = command.Amount;

        school.Modules = command.Modules.ToList();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }

    async Task<Result<string>> ICommandHandler<UpdateSchoolSubscriptionCommand, string>.Handle(UpdateSchoolSubscriptionCommand command, CancellationToken cancellationToken)
    {
        Schools? school = await _context.Schools
            .Include(s => s.Subscriptions)
            .FirstOrDefaultAsync(s => s.PublicId == command.SchoolId, cancellationToken);

        if (school is null)
        {
            return Result.Failure<string>(SchoolErrors.NotFound(command.SchoolId));
        }

        school.Subscriptions.SubscriptionType = command.SubscriptionType;
        school.Subscriptions.StartDate = command.StartDate;
        school.Subscriptions.EndDate = command.EndDate;
        school.Subscriptions.Amount = command.Amount;

        school.Modules = command.Modules.ToList();

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("suscription updated successfully");
    }
}