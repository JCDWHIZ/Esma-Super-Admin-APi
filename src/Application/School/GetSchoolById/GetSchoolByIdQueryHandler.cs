using Application.School.CreateSchool;
using Domain.Schools;

namespace Application.School.GetSchoolById;

public sealed class GetSchoolsWithByIdQueryHandler(IApplicationDbContext _context) : IQueryHandler<GetSchoolByIdQuery, SchoolItemDto>
{

    async Task<Result<SchoolItemDto>> IQueryHandler<GetSchoolByIdQuery, SchoolItemDto>.Handle(GetSchoolByIdQuery query, CancellationToken cancellationToken)
    {
        Schools? entity = await _context.Schools
        .Include(s => s.Subscriptions).FirstOrDefaultAsync(x => x.PublicId == query.PublicId, cancellationToken);
        if (entity == null)
        {
            return Result.Failure<SchoolItemDto>(SchoolErrors.NotFound(query.PublicId));
        }
        var schoolDto = new SchoolItemDto
        {
            PublicId = entity.PublicId,
            SchoolName = entity.SchoolName,
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
            Modules = entity.Modules,
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