using FluentValidation;

namespace Application.School.EditSchool;

public class EditSchoolCommandValidator : AbstractValidator<EditSchoolCommand>
{
    public EditSchoolCommandValidator()
    {
        RuleFor(x => x.PublicId)
            .NotEmpty();

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
        
        RuleFor(x => x.Subscriptions)
            .NotNull();
            
        When(x => x.Subscriptions != null, () => RuleFor(x => x.Subscriptions.SubscriptionType).IsInEnum());

        RuleForEach(x => x.Modules).IsInEnum();
    }
}
