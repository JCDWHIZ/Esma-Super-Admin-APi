using System;
using Application.School.CreateSchool;
using Domain.Schools;

namespace Application.Subscription.GetSubscriptionBySchoolId;

public sealed class GetSubscriptionBySchoolId(IApplicationDbContext _context)
    : IQueryHandler<GetSubscriptionBySchoolIdQuery, SubscriptionDto>
{
    public async Task<Result<SubscriptionDto>> Handle(GetSubscriptionBySchoolIdQuery query, CancellationToken cancellationToken)
    {
        Domain.Schools.Schools? school = await _context.Schools
            .Include(s => s.Subscriptions)
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.PublicId == query.PublicId, cancellationToken);

        if (school is null)
        {
            return Result.Failure<SubscriptionDto>(SchoolErrors.NotFound(query.PublicId));
        }

        var dto = new SubscriptionDto
        {
            SubscriptionType = school.Subscriptions.SubscriptionType,
            StartDate = school.Subscriptions.StartDate,
            EndDate = school.Subscriptions.EndDate,
            Amount = school.Subscriptions.Amount,
            SchoolName = school.SchoolName,
            SchoolLogo = school.LogoUrl,
            Status = Domain.Subscriptions.Subscriptions.GetStatus(school.Subscriptions.StartDate, school.Subscriptions.EndDate),
            SchoolAdminName = $"{school.User.FirstName} {school.User.LastName}",
            SchoolModules = school.Modules
        };

        return Result.Success(dto);
    }
}