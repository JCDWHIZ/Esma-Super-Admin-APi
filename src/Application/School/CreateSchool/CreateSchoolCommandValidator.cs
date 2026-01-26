using Application.Abstractions.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.School.CreateSchool;

public class CreateSchoolCommandValidator : AbstractValidator<CreateSchoolCommand>
{
    private readonly IApplicationDbContext _context;
    public CreateSchoolCommandValidator(IApplicationDbContext context)
    {
        _context = context;
        
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
            RuleFor(x => x.SchoolAdmin.Email).NotEmpty().EmailAddress()
                .MustAsync(async (email, cancellationToken) => 
                    !await _context.Users.AnyAsync(u => u.Email == email, cancellationToken))
                .WithMessage("Email already exists.");
            RuleFor(x => x.SchoolAdmin.Username).NotEmpty()
                .MustAsync(async (username, cancellationToken) => 
                    !await _context.Users.AnyAsync(u => u.Username == username, cancellationToken))
                .WithMessage("Username already exists.");
            RuleFor(x => x.SchoolAdmin.Role).IsInEnum();
        });
        
        RuleFor(x => x.Subscriptions)
            .NotNull();
            
        When(x => x.Subscriptions != null, () => RuleFor(x => x.Subscriptions.SubscriptionType).IsInEnum());

        RuleForEach(x => x.Modules).IsInEnum();
    }
}
