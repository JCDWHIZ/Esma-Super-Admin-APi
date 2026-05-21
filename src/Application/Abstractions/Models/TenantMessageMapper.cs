using Application.School;
using Application.School.CreateSchool;
using Domain.Schools;

namespace Application.Abstractions.Models;

public static class TenantMessageMapper
{
    public static CreateTenantMessage BuildCreateTenantMessage(Schools school)
    {
        return new CreateTenantMessage
        {
            SchoolId = school.Id,
            SchoolPublicId = school.PublicId,
            SchoolName = school.SchoolName,
            OrganizationId = school.OrganizationId ?? string.Empty,
            SchoolAdminEmail = school.User.Email,
            SchoolAdminFirstName = school.User.FirstName,
            SchoolAdminLastName = school.User.LastName,
            SchoolAdminUsername = school.User.Username,
            SchoolAdminRole = school.User.Role,
            SchoolLogoUrl = school.LogoUrl,
            SchoolEmail = school.EmailAddress,
            SchoolPhoneNumber = school.PhoneNumber,
            SchoolAdminPhoneNumber = school.User.PhoneNumber,
            SchoolAddress = school.Address == null ? null : new AddressDto
            {
                State = school.Address.State ?? string.Empty,
                Country = school.Address.Country ?? string.Empty,
                Lga = school.Address.LGA ?? string.Empty,
                StreetAddress = school.Address.StreetAddress ?? string.Empty
            },
            Modules = school.Modules.Select(m => m.Key).ToList(),
            Subscriptions = new SubscriptionDto
            {
                SubscriptionType = school.Subscriptions.SubscriptionType,
                StartDate = school.Subscriptions.StartDate,
                EndDate = school.Subscriptions.EndDate,
                Amount = school.Subscriptions.Amount
            }
        };
    }

    public static UpdateTenantPayload BuildUpdateTenantPayload(Schools school)
    {
        CreateTenantMessage payload = BuildCreateTenantMessage(school);
        return new UpdateTenantPayload
        {
            SchoolId = payload.SchoolId,
            SchoolPublicId = payload.SchoolPublicId,
            SchoolName = payload.SchoolName,
            OrganizationId = payload.OrganizationId,
            SchoolAdminEmail = payload.SchoolAdminEmail,
            SchoolAdminFirstName = payload.SchoolAdminFirstName,
            SchoolAdminLastName = payload.SchoolAdminLastName,
            SchoolAdminUsername = payload.SchoolAdminUsername,
            SchoolAdminPhoneNumber = payload.SchoolAdminPhoneNumber,
            SchoolAdminRole = payload.SchoolAdminRole,
            SchoolLogoUrl = payload.SchoolLogoUrl,
            SchoolEmail = payload.SchoolEmail,
            SchoolPhoneNumber = payload.SchoolPhoneNumber,
            SchoolAddress = payload.SchoolAddress,
            Modules = payload.Modules,
            Subscriptions = payload.Subscriptions,
            TenantId = school.TenantId ?? string.Empty
        };
    }
}
