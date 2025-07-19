using System;
using Application.School.CreateSchool;

namespace Application.School.GetDeletedSchoolWithPagination;

public sealed class GetDeletedSchoolsWithPaginationQueryHandler(IApplicationDbContext _context) : IQueryHandler<GetDeletedSchoolsWithPaginationQuery, PaginatedList<SchoolItemDto>>
{
    async Task<Result<PaginatedList<SchoolItemDto>>> IQueryHandler<GetDeletedSchoolsWithPaginationQuery, PaginatedList<SchoolItemDto>>.Handle(GetDeletedSchoolsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Domain.Schools.Schools> Schoolquery = _context.Schools.AsQueryable().Where(e => e.IsDeleted).IgnoreQueryFilters();

        if (!string.IsNullOrEmpty(query.SchoolName))
        {
            Schoolquery = Schoolquery.Where(x => x.SchoolName.Contains(query.SchoolName));
        }

        if (!string.IsNullOrEmpty(query.PhoneNumber))
        {
            Schoolquery = Schoolquery.Where(x => x.PhoneNumber.Contains(query.PhoneNumber));
        }

        if (!string.IsNullOrEmpty(query.EmailAddress))
        {
            Schoolquery = Schoolquery.Where(x => x.EmailAddress.Contains(query.EmailAddress));
        }

        if (!string.IsNullOrEmpty(query.LogoUrl))
        {
            Schoolquery = Schoolquery.Where(x => x.LogoUrl.Contains(query.LogoUrl));
        }

        if (!string.IsNullOrEmpty(query.AddressCountry))
        {
            Schoolquery = Schoolquery.Where(x => x.Address.Country == query.AddressCountry);
        }

        if (!string.IsNullOrEmpty(query.AddressState))
        {
            Schoolquery = Schoolquery.Where(x => x.Address.State == query.AddressState);
        }

        if (!string.IsNullOrEmpty(query.AddressLga))
        {
            Schoolquery = Schoolquery.Where(x => x.Address.LGA == query.AddressLga);
        }

        if (!string.IsNullOrEmpty(query.AddressStreetAddress))
        {
            Schoolquery = Schoolquery.Where(x => x.Address.StreetAddress == query.AddressStreetAddress);
        }


        if (query.Subscribed.HasValue)
        {
            Schoolquery = Schoolquery.Where(x => x.Subscribed == query.Subscribed.Value);
        }

        if (query.Status.HasValue)
        {
            Schoolquery = Schoolquery.Where(x => x.Status == query.Status.Value);
        }

        if (query.SubscriptionType.HasValue)
        {
            Schoolquery = Schoolquery.Where(x => x.Subscriptions.SubscriptionType == query.SubscriptionType.Value);
        }

        PaginatedList<SchoolItemDto> schoolList = await PaginatedList<SchoolItemDto>.CreateAsync(
            Schoolquery.Select(s => new SchoolItemDto
            {
                PublicId = s.PublicId,
                SchoolName = s.SchoolName,
                LogoUrl = s.LogoUrl,
                Address = s.Address == null ? null : new AddressDto
                {
                    State = s.Address.State ?? string.Empty,
                    Country = s.Address.Country ?? string.Empty,
                    Lga = s.Address.LGA ?? string.Empty,
                    StreetAddress = s.Address.StreetAddress ?? string.Empty
                },
                EmailAddress = s.EmailAddress,
                PhoneNumber = s.PhoneNumber,
                DocumentUrl = s.DocumentUrl,
                Modules = s.Modules,
                Subscribed = s.Subscribed,
                Status = s.Status,
                Subscriptions = s.Subscriptions == null ? null : new SubscriptionDto
                {
                    SubscriptionType = s.Subscriptions.SubscriptionType,
                    StartDate = s.Subscriptions.StartDate ?? DateTime.MinValue,
                    EndDate = s.Subscriptions.EndDate ?? DateTime.MinValue,
                    Amount = s.Subscriptions.Amount
                },
                User = s.User == null ? null : new SchoolAdmins
                {
                    Role = s.User.Role,
                    Username = s.User.Username,
                    FirstName = s.User.FirstName,
                    LastName = s.User.LastName,
                    Email = s.User.Email,
                    PhoneNumber = s.User.PhoneNumber
                },
            }),
            query.PageNumber ?? 1,
            query.PageSize ?? 10
        );

        return Result.Success(schoolList);
    }
}
