using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.School.EditSchool;

public class EditSchoolCommandValidator : AbstractValidator<EditSchoolCommand>
{
    private readonly IApplicationDbContext _context;
    public EditSchoolCommandValidator(IApplicationDbContext context)
    {
        _context = context;
        RuleFor(x => x.PublicId)
            .NotEmpty();

        RuleFor(x => x.SchoolName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.ShortCode)
            .NotEmpty()
            .MaximumLength(50)
            .MustAsync(async (command, shortCode, cancellationToken) =>
            {
                string normalized = shortCode.Trim().ToUpperInvariant();
                return !await _context.Schools.AnyAsync(
                    s => s.PublicId != command.PublicId && s.ShortCode == normalized,
                    cancellationToken);
            })
            .WithMessage("Short code already exists.");

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

        RuleFor(x => x.Modules)
            .Must(modules => modules.Count > 0)
            .WithMessage("At least one module key must be provided.");

        RuleFor(x => x.Modules)
            .MustAsync(async (modules, cancellationToken) =>
            {
                var normalized = modules
                    .Where(m => !string.IsNullOrWhiteSpace(m))
                    .Select(m => m.Trim().ToUpperInvariant())
                    .Distinct()
                    .ToList();

                if (normalized.Count != modules.Count)
                {
                    return false;
                }

                int count = await _context.SchoolModules
                    .CountAsync(m => normalized.Contains(m.Key), cancellationToken);

                return count == normalized.Count;
            })
            .WithMessage("One or more module keys are invalid.");
    }
}
