
using Domain.Schools;
using Domain.Subscriptions;
using Application.Abstractions.Models;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.School.EditSchool;



public sealed class EditSchoolCommandCommandHandler(
    IApplicationDbContext _dbContext,
    IMessageProducer _messageProducer,
    IConfiguration _configuration,
    ILogger<EditSchoolCommandCommandHandler> _logger) : ICommandHandler<EditSchoolCommand, string>
{
    async Task<Result<string>> ICommandHandler<EditSchoolCommand, string>.Handle(EditSchoolCommand command, CancellationToken cancellationToken)
    {
        Schools? entity = await _dbContext.Schools
                .Include(s => s.Subscriptions)
                .Include(s => s.Modules)
                .Include(s => s.User)
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

        if (string.IsNullOrWhiteSpace(entity.TenantId))
        {
            _logger.LogWarning("Skipping UpdateTenant publish for school {SchoolPublicId} because TenantId is empty", entity.PublicId);
            return Result.Success("School updated successfully");
        }

        UpdateTenantPayload updateTenantPayload = TenantMessageMapper.BuildUpdateTenantPayload(entity);
        await _messageProducer.SendMessageAsync(
            "UpdateTenant",
            updateTenantPayload,
            _configuration["Kafka:CreateTenantTopic"]);

        return Result.Success("School updated successfully");
    }
}
