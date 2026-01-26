using FluentValidation;

namespace Application.School.CreateSchool;

public class CreateSchoolCommandValidator : AbstractValidator<CreateSchoolCommand>
{
    public CreateSchoolCommandValidator()
    {
        RuleFor(x => x.SchoolName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty();
        
        RuleFor(x => x.Address)
            .NotNull();
        
        When(x => x.Address != null, () =>
        {
            RuleFor(x => x.Address.State).NotEmpty();
            RuleFor(x => x.Address.Country).NotEmpty();
            RuleFor(x => x.Address.Lga).NotEmpty();
            RuleFor(x => x.Address.StreetAddress).NotEmpty();
        });

        RuleFor(x => x.SchoolAdmin)
            .NotNull();

        When(x => x.SchoolAdmin != null, () =>
        {
            RuleFor(x => x.SchoolAdmin.FirstName).NotEmpty();
            RuleFor(x => x.SchoolAdmin.LastName).NotEmpty();
            RuleFor(x => x.SchoolAdmin.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.SchoolAdmin.Username).NotEmpty();
        });
        
        RuleFor(x => x.Subscriptions)
            .NotNull();
            
        When(x => x.Subscriptions != null, () =>
        {
            RuleFor(x => x.Subscriptions.SubscriptionType).IsInEnum();
        });
    }
}
