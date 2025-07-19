// using System;
// using admin_service.Domain.Entities;
// using FluentValidation;

// namespace admin_service.Application.School.Queries.GetSchoolsWithPagination;

// public class GetSchoolsWithPaginationQueryValidator: AbstractValidator<GetSchoolsWithPaginationQuery>
// {
//     public GetSchoolsWithPaginationQueryValidator()
//     {
//         RuleFor(x => x.SchoolName)
//             .MaximumLength(100).WithMessage("School name must not exceed 100 characters.");

//         RuleFor(x => x.LogoUrl)
//             .Matches(@"^(http|https)://").When(x => !string.IsNullOrEmpty(x.LogoUrl))
//             .WithMessage("Logo URL must be a valid URL starting with http or https.");

//         // RuleFor(x => x.Address)
//         //     .Must(BeAValidAddress).When(x => x.Address != null)
//         //     .WithMessage("Address fields must not exceed their respective limits.");

//         RuleFor(x => x.EmailAddress)
//             .EmailAddress().WithMessage("Email address is not valid.");

//         RuleFor(x => x.PhoneNumber)
//             .Matches(@"^\+?[0-9\s\-]*$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
//             .WithMessage("Phone number must be in a valid format.");

//         // RuleFor(x => x.DocumentUrl)
//         //     .Must(docs => docs != null && docs.All(doc => !string.IsNullOrEmpty(doc)))
//         //     .WithMessage("Each document URL must be a valid, non-empty string.")
//         //     .When(x => x.DocumentUrl != null && x.DocumentUrl.Any());

//         // RuleFor(x => x.Modules)
//         //     .Must(modules => modules != null && modules.All(m => !string.IsNullOrEmpty(m.ToString())))
//         //     .WithMessage("Each module must have a valid name.")
//         //     .When(x => x.Modules != null && x.Modules.Any());

//         RuleFor(x => x.PageNumber)
//             .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

//         RuleFor(x => x.PageSize)
//             .GreaterThanOrEqualTo(1).WithMessage("Page size must be at least 1.")
//             .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");
//     }
//    private bool BeAValidAddress(Address? address)
//     {
//         // If address is null, it's valid (the rule only runs when it's non-null)
//         if (address == null)
//             return true;

//         // You can enforce your own rules here (for example, at least one property must be provided)
//         return !string.IsNullOrWhiteSpace(address.Country) ||
//                !string.IsNullOrWhiteSpace(address.State) ||
//                !string.IsNullOrWhiteSpace(address.LGA) ||
//                !string.IsNullOrWhiteSpace(address.StreetAddress);
//     }
// }
