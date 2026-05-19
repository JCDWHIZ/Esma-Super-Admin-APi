
using Domain.Schools;
using Domain.Subscriptions;

namespace Application.School.EditSchool;



public sealed class EditSchoolCommandCommandHandler(IApplicationDbContext _dbContext) : ICommandHandler<EditSchoolCommand, string>
{
    async Task<Result<string>> ICommandHandler<EditSchoolCommand, string>.Handle(EditSchoolCommand command, CancellationToken cancellationToken)
    {
        Schools? entity = await _dbContext.Schools
                .Include(s => s.Subscriptions)
                .Include(s => s.Modules)
                .FirstOrDefaultAsync(s => s.PublicId == command.PublicId, cancellationToken);

        if (entity == null)
        {
            return Result.Failure<string>(SchoolErrors.NotFound(command.PublicId));
        }

        entity.SchoolName = command.SchoolName;
        entity.LogoUrl = command.LogoUrl;
        entity.Address = new Address
        {
            State = command.Address.State,
            Country = command.Address.Country,
            LGA = command.Address.Lga,
            StreetAddress = command.Address.StreetAddress
        };
        entity.EmailAddress = command.EmailAddress;
        entity.PhoneNumber = command.PhoneNumber;
        var moduleKeys = command.Modules
            .Select(m => m.Trim().ToUpperInvariant())
            .Distinct()
            .ToList();
        List<SchoolModule> modules = await _dbContext.SchoolModules
            .Where(m => moduleKeys.Contains(m.Key))
            .ToListAsync(cancellationToken);
        if (modules.Count != moduleKeys.Count)
        {
            return Result.Failure<string>(SchoolErrors.InvalidModuleKeys);
        }
        entity.Modules = modules;
        entity.DocumentUrl = command.DocumentUrl;

        entity.Subscriptions ??= new Subscriptions();

        entity.Subscriptions.SubscriptionType = command.Subscriptions.SubscriptionType;
        entity.Subscriptions.StartDate = command.Subscriptions.StartDate;
        entity.Subscriptions.EndDate = command.Subscriptions.EndDate;
        entity.Subscriptions.Amount = command.Subscriptions.Amount;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success("School updated successfully");
    }
}
