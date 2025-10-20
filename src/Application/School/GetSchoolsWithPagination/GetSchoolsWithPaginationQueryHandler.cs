using Application.School.CreateSchool;
using FluentValidation;

namespace Application.School.GetSchoolsWithPagination;

public sealed class GetShoolsWithPaginationQueryHandler(IApplicationDbContext _context) : IQueryHandler<GetSchoolsWithPaginationQuery, PaginatedList<SchoolItemDto>>
{
    async Task<Result<PaginatedList<SchoolItemDto>>> IQueryHandler<GetSchoolsWithPaginationQuery, PaginatedList<SchoolItemDto>>.Handle(GetSchoolsWithPaginationQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Domain.Schools.Schools> schoolQuery = _context.Schools.AsQueryable();

        if (!string.IsNullOrEmpty(query.SchoolName))
        {
            schoolQuery = schoolQuery.Where(x => x.SchoolName.Contains(query.SchoolName));
        }

        if (!string.IsNullOrEmpty(query.PhoneNumber))
        {
            schoolQuery = schoolQuery.Where(x => x.PhoneNumber.Contains(query.PhoneNumber));
        }

        if (!string.IsNullOrEmpty(query.EmailAddress))
        {
            schoolQuery = schoolQuery.Where(x => x.EmailAddress.Contains(query.EmailAddress));
        }

        if (!string.IsNullOrEmpty(query.LogoUrl))
        {
            schoolQuery = schoolQuery.Where(x => x.LogoUrl.Contains(query.LogoUrl));
        }

        if (!string.IsNullOrEmpty(query.AddressCountry))
        {
            schoolQuery = schoolQuery.Where(x => x.Address.Country == query.AddressCountry);
        }

        if (!string.IsNullOrEmpty(query.AddressState))
        {
            schoolQuery = schoolQuery.Where(x => x.Address.State == query.AddressState);
        }

        if (!string.IsNullOrEmpty(query.AddressLga))
        {
            schoolQuery = schoolQuery.Where(x => x.Address.LGA == query.AddressLga);
        }

        if (!string.IsNullOrEmpty(query.AddressStreetAddress))
        {
            schoolQuery = schoolQuery.Where(x => x.Address.StreetAddress == query.AddressStreetAddress);
        }


        if (query.Subscribed.HasValue)
        {
            schoolQuery = schoolQuery.Where(x => x.Subscribed == query.Subscribed.Value);
        }

        if (query.Status.HasValue)
        {
            schoolQuery = schoolQuery.Where(x => x.Status == query.Status.Value);
        }

        if (query.SubscriptionType.HasValue)
        {
            schoolQuery = schoolQuery.Where(x => x.Subscriptions.SubscriptionType == query.SubscriptionType.Value);
        }

        PaginatedList<SchoolItemDto> schoolList = await PaginatedList<SchoolItemDto>.CreateAsync(
           schoolQuery.Select(s => new SchoolItemDto
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
               User = s.User == null ? null : new UserDto
               {
                   Role = s.User.Role,
                   Username = s.User.Username,
                   FirstName = s.User.FirstName,
                   LastName = s.User.LastName,
                   Email = s.User.Email,
                   PhoneNumber = s.User.PhoneNumber ?? string.Empty
               },
           }),
           query.PageNumber ?? 1,
           query.PageSize ?? 10
       );

        return Result.Success(schoolList);
    }
}