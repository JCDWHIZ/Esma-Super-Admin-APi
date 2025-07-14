using admin_service.Domain.Entities;

namespace Microsoft.Extensions.DependencyInjection.School.Commands;

public class InitiateSchoolRequestValidator : AbstractValidator<InitiateSchoolRequestCommand>
{
    public InitiateSchoolRequestValidator()
    {
        // RuleFor(x => x.SchoolName)
        //     .NotEmpty().WithMessage("School name is required.")
        //     .MaximumLength(100).WithMessage("School name must not exceed 100 characters.");

        // RuleFor(x => x.LogoUrl)
        //     .Matches(@"^(http|https)://").When(x => !string.IsNullOrEmpty(x.LogoUrl))
        //     .WithMessage("Logo URL must be a valid URL starting with http or https.");

        // RuleFor(x => x.Address)
        //     .Must(BeAValidAddress).When(x => x.Address != null)
        //     .WithMessage("Address fields must not exceed their respective limits.");

        // RuleFor(x => x.EmailAddress)
        //     .EmailAddress().WithMessage("Email address is not valid.");

        // RuleFor(x => x.PhoneNumber)
        //     .Matches(@"^\+?[0-9\s\-]*$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
        //     .WithMessage("Phone number must be a valid format.");

        // RuleFor(x => x.DocumentUrl)
        //     .Must(docs => docs != null && docs.All(doc => !string.IsNullOrEmpty(doc)))
        //     .WithMessage("Each document must have a valid URL.")
        //     .When(x => x.DocumentUrl != null && x.DocumentUrl.Any());

        // RuleFor(x => x.Modules)
        //     .Must(modules => modules != null && modules.All(m => !string.IsNullOrEmpty(m.ToString())))
        //     .WithMessage("Each module must have a valid name.")
        //     .When(x => x.Modules != null && x.Modules.Any());
        // RuleFor(x => x.Subscriptions)
        //     .Must(subscriptions => subscriptions != null)
        //     .WithMessage("Subscriptions must not be null.");

         RuleFor(x => x.SchoolName).NotEmpty().WithMessage("School name is required");
        RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress().WithMessage("Valid email is required");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required");
        RuleFor(x => x.Address).NotNull().WithMessage("Address is required");
        RuleFor(x => x.Subscriptions).NotNull().WithMessage("Subscription details are required");
        RuleFor(x => x.Subscriptions.StartDate).NotEmpty().WithMessage("Subscription start date is required");
        RuleFor(x => x.Subscriptions.EndDate).NotEmpty().WithMessage("Subscription end date is required");
    }

    private bool BeAValidAddress(Address address)
    {
        return (string.IsNullOrEmpty(address.Country) || address.Country.Length <= 100) &&
               (string.IsNullOrEmpty(address.State) || address.State.Length <= 100) &&
               (string.IsNullOrEmpty(address.LGA) || address.LGA.Length <= 100) &&
               (string.IsNullOrEmpty(address.StreetAddress) || address.StreetAddress.Length <= 250);
    }
}
