using Application.School.CreateSchool;
using Domain.Schools;

namespace Application.School.GetSchoolById;

public sealed class GetSchoolsWithByIdQueryHandler(IApplicationDbContext _context) : IQueryHandler<GetSchoolByIdQuery, SchoolItemDto>
{

    async Task<Result<SchoolItemDto>> IQueryHandler<GetSchoolByIdQuery, SchoolItemDto>.Handle(GetSchoolByIdQuery query, CancellationToken cancellationToken)
    {
        Schools? entity = await _context.Schools
        .Include(s => s.Subscriptions)
        .Include(s => s.User)
        .Include(s => s.Modules)
        .Include(s => s.SisModules)
        .Include(s => s.LmsModules)
        .FirstOrDefaultAsync(x => x.PublicId == query.PublicId, cancellationToken);
        if (entity == null)
        {
            return Result.Failure<SchoolItemDto>(SchoolErrors.NotFound(query.PublicId));
        }

        var assignedModuleIds = entity.Modules.Select(sm => sm.Id).ToHashSet();
        var assignedSisModuleIds = entity.SisModules.Select(sm => sm.Id).ToHashSet();
        var assignedLmsModuleIds = entity.LmsModules.Select(sm => sm.Id).ToHashSet();

        List<SchoolModuleAvailabilityDto> allModules = await _context.SchoolModules
            .Select(m => new SchoolModuleAvailabilityDto
            {
                Name = m.Name,
                Key = m.Key,
                Description = m.Description,
                HasModule = assignedModuleIds.Contains(m.Id)
            })
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);

        List<SisModuleAvailabilityDto> allSisModules = await _context.SisModules
            .Select(m => new SisModuleAvailabilityDto
            {
                Name = m.Name,
                Key = m.Key,
                Description = m.Description,
                HasModule = assignedSisModuleIds.Contains(m.Id)
            })
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);

        List<LmsModuleAvailabilityDto> allLmsModules = await _context.LmsModules
            .Select(m => new LmsModuleAvailabilityDto
            {
                Name = m.Name,
                Key = m.Key,
                Description = m.Description,
                HasModule = assignedLmsModuleIds.Contains(m.Id)
            })
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);

        var schoolDto = new SchoolItemDto
        {
            PublicId = entity.PublicId,
            SchoolName = entity.SchoolName,
            ShortCode = entity.ShortCode,
            LogoUrl = entity.LogoUrl,
            EmailAddress = entity.EmailAddress,
            PhoneNumber = entity.PhoneNumber,
            Address = entity.Address == null ? null : new AddressDto
            {
                State = entity.Address.State ?? string.Empty,
                Country = entity.Address.Country ?? string.Empty,
                Lga = entity.Address.LGA ?? string.Empty,
                StreetAddress = entity.Address.StreetAddress ?? string.Empty
            },
            Subscriptions = entity.Subscriptions == null ? null : new SubscriptionDto
            {
                SubscriptionType = entity.Subscriptions.SubscriptionType,
                StartDate = entity.Subscriptions.StartDate ?? DateTime.MinValue,
                EndDate = entity.Subscriptions.EndDate ?? DateTime.MinValue,
                Amount = entity.Subscriptions.Amount
            },
            Modules = entity.Modules.Select(m => new SchoolModuleResponseDto
            {
                Name = m.Name,
                Key = m.Key,
                Description = m.Description
            }).ToList(),
            ModuleAvailability = allModules,
            SisModules = entity.SisModules.Select(m => new SisModuleResponseDto
            {
                Name = m.Name,
                Key = m.Key,
                Description = m.Description
            }).ToList(),
            SisModuleAvailability = allSisModules,
            LmsModules = entity.LmsModules.Select(m => new LmsModuleResponseDto
            {
                Name = m.Name,
                Key = m.Key,
                Description = m.Description
            }).ToList(),
            LmsModuleAvailability = allLmsModules,
            DocumentUrl = entity.DocumentUrl,
            User = entity.User == null ? null :
            new UserDto
            {
                Role = entity.User.Role,
                Username = entity.User.Username,
                FirstName = entity.User.FirstName,
                LastName = entity.User.LastName,
                Email = entity.User.Email,
                PhoneNumber = entity.User.PhoneNumber ?? string.Empty
            },
        };
        return schoolDto;
    }
}
