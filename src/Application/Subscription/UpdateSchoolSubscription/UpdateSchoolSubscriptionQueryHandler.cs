using Domain.Schools;
using Application.Abstractions.Models;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Subscription.UpdateSchoolSubscription;

public sealed class UpdateSchoolSubscriptionHandler(
    IApplicationDbContext _context,
    IMessageProducer _messageProducer,
    IConfiguration _configuration,
    ILogger<UpdateSchoolSubscriptionHandler> _logger)
    : ICommandHandler<UpdateSchoolSubscriptionCommand, string>
{
    public async Task<Result<bool>> Handle(UpdateSchoolSubscriptionCommand command, CancellationToken cancellationToken)
    {
        Schools? school = await _context.Schools
            .Include(s => s.Subscriptions)
            .Include(s => s.Modules)
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.PublicId == command.SchoolId, cancellationToken);

        if (school is null)
        {
            return Result.Failure<bool>(SchoolErrors.NotFound(command.SchoolId));
        }

        school.Subscriptions.SubscriptionType = command.SubscriptionType;
        school.Subscriptions.StartDate = command.StartDate;
        school.Subscriptions.EndDate = command.EndDate;
        school.Subscriptions.Amount = command.Amount;

        var moduleKeys = command.Modules
            .Select(m => m.Trim().ToUpperInvariant())
            .Distinct()
            .ToList();
        List<SchoolModule> modules = await _context.SchoolModules
            .Where(m => moduleKeys.Contains(m.Key))
            .ToListAsync(cancellationToken);
        if (modules.Count != moduleKeys.Count)
        {
            return Result.Failure<bool>(SchoolErrors.InvalidModuleKeys);
        }
        school.Modules = modules;

        await _context.SaveChangesAsync(cancellationToken);
        await PublishUpdateTenantMessageAsync(school);

        return Result.Success(true);
    }

    async Task<Result<string>> ICommandHandler<UpdateSchoolSubscriptionCommand, string>.Handle(UpdateSchoolSubscriptionCommand command, CancellationToken cancellationToken)
    {
        Schools? school = await _context.Schools
            .Include(s => s.Subscriptions)
            .Include(s => s.Modules)
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.PublicId == command.SchoolId, cancellationToken);

        if (school is null)
        {
            return Result.Failure<string>(SchoolErrors.NotFound(command.SchoolId));
        }

        school.Subscriptions.SubscriptionType = command.SubscriptionType;
        school.Subscriptions.StartDate = command.StartDate;
        school.Subscriptions.EndDate = command.EndDate;
        school.Subscriptions.Amount = command.Amount;

        var moduleKeys = command.Modules
            .Select(m => m.Trim().ToUpperInvariant())
            .Distinct()
            .ToList();
        List<SchoolModule> modules = await _context.SchoolModules
            .Where(m => moduleKeys.Contains(m.Key))
            .ToListAsync(cancellationToken);
        if (modules.Count != moduleKeys.Count)
        {
            return Result.Failure<string>(SchoolErrors.InvalidModuleKeys);
        }
        school.Modules = modules;

        await _context.SaveChangesAsync(cancellationToken);
        await PublishUpdateTenantMessageAsync(school);

        return Result.Success("suscription updated successfully");
    }

    private async Task PublishUpdateTenantMessageAsync(Schools school)
    {
        if (string.IsNullOrWhiteSpace(school.TenantId))
        {
            _logger.LogWarning("Skipping UpdateTenant publish for school {SchoolPublicId} because TenantId is empty", school.PublicId);
            return;
        }

        UpdateTenantPayload updateTenantPayload = TenantMessageMapper.BuildUpdateTenantPayload(school);
        await _messageProducer.SendMessageAsync(
            "UpdateTenant",
            updateTenantPayload,
            _configuration["Kafka:CreateTenantTopic"]);
    }
}
