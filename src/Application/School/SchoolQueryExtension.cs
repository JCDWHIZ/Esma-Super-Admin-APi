// File: SchoolQueryExtensions.cs

using admin_service.Domain.Entities;
using admin_service.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace admin_service.Application.School.Queries
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(
            this IQueryable<T> query,
            bool condition,
            Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }
    }

    public static class SchoolQueryExtensions
    {

        
        // public static IQueryable<Schools> ApplySchoolFilters(
        //     this IQueryable<Schools> query,
        //     string? schoolName,
        //     string? phoneNumber,
        //     string? emailAddress,
        //     string? logoUrl,
        //     Address? address,
        //     // ICollection<string>? documentUrl,
        //     // ICollection<Modules> modules,
        //     bool? subscribed,
        //     SchoolStatus? status,
        //     SubscriptionType? subscriptionType)
        // {
        //     if (!string.IsNullOrEmpty(schoolName))
        //     {
        //         query = query.Where(x => x.SchoolName.Contains(schoolName));
        //     }

        //     if (!string.IsNullOrEmpty(phoneNumber))
        //     {
        //         query = query.Where(x => x.PhoneNumber == phoneNumber);
        //     }

        //     if (!string.IsNullOrEmpty(emailAddress))
        //     {
        //         query = query.Where(x => x.EmailAddress.Contains(emailAddress));
        //     }

        //     if (!string.IsNullOrEmpty(logoUrl))
        //     {
        //         query = query.Where(x => x.LogoUrl.Contains(logoUrl));
        //     }

        //     if (address != null)
        //     {
        //         if (!string.IsNullOrEmpty(address.Country))
        //         {
        //             query = query.Where(x => x.Address.Country == address.Country);
        //         }

        //         if (!string.IsNullOrEmpty(address.State))
        //         {
        //             query = query.Where(x => x.Address.State == address.State);
        //         }

        //         if (!string.IsNullOrEmpty(address.LGA))
        //         {
        //             query = query.Where(x => x.Address.LGA == address.LGA);
        //         }

        //         if (!string.IsNullOrEmpty(address.StreetAddress))
        //         {
        //             query = query.Where(x => x.Address.StreetAddress == address.StreetAddress);
        //         }
        //     }

        //     // if (documentUrl != null && documentUrl.Any())
        //     // {
        //     //     query = query.Where(x => documentUrl.All(doc => x.DocumentUrl.Contains(doc)));
        //     // }

        //     // if (modules != null && modules.Any())
        //     // {
        //     //     query = query.Where(x => modules.All(m => x.Modules.Contains(m)));
        //     // }

        //     if (subscribed.HasValue)
        //     {
        //         query = query.Where(x => x.Subscribed == subscribed);
        //     }

        //     if (status.HasValue)
        //     {
        //         query = query.Where(x => x.Status == status);
        //     }

        //     if (subscriptionType.HasValue)
        //     {
        //         query = query.Where(x => x.Subscriptions.subscriptionType == subscriptionType);
        //     }

        //     return query;
        // }

         public static IQueryable<Schools> ApplySchoolFilters(
            this IQueryable<Schools> query,
            string? schoolName,
            string? phoneNumber,
            string? emailAddress,
            string? logoUrl,
            Address? address,
            bool? subscribed,
            SchoolStatus? status,
            SubscriptionType? subscriptionType)
        {
            query = query
                .WhereIf(!string.IsNullOrWhiteSpace(schoolName),
                    x => x.SchoolName.Contains(schoolName!))
                .WhereIf(!string.IsNullOrWhiteSpace(phoneNumber),
                    x => x.PhoneNumber == phoneNumber)
                .WhereIf(!string.IsNullOrWhiteSpace(emailAddress),
                    x => x.EmailAddress.Contains(emailAddress!))
                .WhereIf(!string.IsNullOrWhiteSpace(logoUrl),
                    x => x.LogoUrl.Contains(logoUrl!));

            if (address != null)
            {
                query = query
                .WhereIf(!string.IsNullOrWhiteSpace(address.Country),
                    x => x.Address.Country == address.Country!)
                .WhereIf(!string.IsNullOrWhiteSpace(address.State),
                    x => x.Address.State == address.State!)
                .WhereIf(!string.IsNullOrWhiteSpace(address.LGA),
                    x => x.Address.LGA == address.LGA!)
                .WhereIf(!string.IsNullOrWhiteSpace(address.StreetAddress),
                    x => x.Address.StreetAddress == address.StreetAddress!);
            }

            query = query
                .WhereIf(subscribed.HasValue, x => x.Subscribed == subscribed)
                .WhereIf(status.HasValue, x => x.Status == status)
                .WhereIf(subscriptionType.HasValue, 
                    x => x.Subscriptions.subscriptionType == subscriptionType);

            return query;
        }
    }
}


