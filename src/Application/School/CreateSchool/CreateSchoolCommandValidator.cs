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

        RuleFor(x => x.ShortCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(50)
            .MustAsync(async (shortCode, cancellationToken) =>
            {
                string normalized = shortCode.Trim().ToUpperInvariant();
                return !await _context.Schools.AnyAsync(
                    s => s.ShortCode == normalized,
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

        RuleFor(x => x.SchoolAdmin)
            .NotNull();

        When(x => x.SchoolAdmin != null, () =>
        {
            RuleFor(x => x.SchoolAdmin.FirstName).NotEmpty();
            RuleFor(x => x.SchoolAdmin.LastName).NotEmpty();
            RuleFor(x => x.SchoolAdmin.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .EmailAddress()
                .MustAsync(async (email, cancellationToken) =>
                {
                    string normalized = email.Trim();
                    return !await _context.SchoolAdmins.AnyAsync(
                        schoolAdmin => schoolAdmin.Email == normalized,
                        cancellationToken);
                })
                .WithMessage("School admin email already exists.");

            RuleFor(x => x.SchoolAdmin.Username)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (username, cancellationToken) =>
                {
                    string normalized = username.Trim();
                    return !await _context.SchoolAdmins.AnyAsync(
                        schoolAdmin => schoolAdmin.Username == normalized,
                        cancellationToken);
                })
                .WithMessage("School admin username already exists.");
            RuleFor(x => x.SchoolAdmin.Role).IsInEnum();
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

        RuleFor(x => x.SisModules)
            .MustAsync(async (modules, cancellationToken) =>
            {
                if (modules.Count == 0)
                {
                    return true;
                }

                var normalized = modules
                    .Where(m => !string.IsNullOrWhiteSpace(m))
                    .Select(m => m.Trim().ToUpperInvariant())
                    .Distinct()
                    .ToList();

                if (normalized.Count != modules.Count)
                {
                    return false;
                }

                int count = await _context.SisModules
                    .CountAsync(m => normalized.Contains(m.Key), cancellationToken);

                return count == normalized.Count;
            })
            .WithMessage("One or more SIS module keys are invalid.");

        RuleFor(x => x.LmsModules)
            .MustAsync(async (modules, cancellationToken) =>
            {
                if (modules.Count == 0)
                {
                    return true;
                }

                var normalized = modules
                    .Where(m => !string.IsNullOrWhiteSpace(m))
                    .Select(m => m.Trim().ToUpperInvariant())
                    .Distinct()
                    .ToList();

                if (normalized.Count != modules.Count)
                {
                    return false;
                }

                int count = await _context.LmsModules
                    .CountAsync(m => normalized.Contains(m.Key), cancellationToken);

                return count == normalized.Count;
            })
            .WithMessage("One or more LMS module keys are invalid.");
    }
}
