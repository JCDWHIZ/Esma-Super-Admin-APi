using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Models;
using Application.Auth.Login;
using Application.Interfaces;
using Domain.Schools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.School.ResendSchoolResetLink;

public sealed record ResendSchoolResetLinkCommand(Guid SchoolPublicId) : ICommand<string>;
public class ResendSchoolResetLinkCommandHandler(IApplicationDbContext context, ILogger<ResendSchoolResetLinkCommandHandler> _logger, IEmailService _emailService, ITokenService _tokenService, IConfiguration _configuration)
    : ICommandHandler<ResendSchoolResetLinkCommand, string>
{
    public async Task<Result<string>> Handle(ResendSchoolResetLinkCommand command, CancellationToken cancellationToken)
    {
        Schools? school = await context.Schools
           .Include(s => s.User)
           .FirstOrDefaultAsync(s => s.PublicId == command.SchoolPublicId, cancellationToken);

        if (school is null)
        {
            return Result.Failure<string>(SchoolErrors.NotFound(command.SchoolPublicId));
        }

        if (school.TenantId is null)
        {
            return Result.Failure<string>(SchoolErrors.NotFoundTenantId);
        }

        try
        {
            var payload = new Dictionary<string, object>
            {
                { "schoolId", school.Id },
                { "schoolPublicId", school.PublicId },
                { "organizationId", school.OrganizationId ?? string.Empty },
                { "schoolName", school.SchoolName },
                { "email", school.User.Email },
                { "firstName", school.User.FirstName },
                { "lastName", school.User.LastName },
                { "role", school.User.Role.ToString() },
                { "username", school.User.Username },
                { "phoneNumber", school.User?.PhoneNumber ?? string.Empty },
                { "tenantId", school.TenantId }
            };

            string token = _tokenService.GenerateToken(payload);

            var emailMessage = new EmailMessage
            {
                Email = school.User?.Email ?? school.EmailAddress,
                Title = "Your School Organization is Ready",
                Name = school.SchoolName,
                Description = "We've successfully onboarded your school to our platform. We're excited to share that your school has been successfully added to our platform! This marks the beginning of a seamless, integrated experience designed to empower your institution with the tools and support needed to thrive. Welcome aboard-we're looking forward to growing with you.",
                EmailButton = true,
                ButtonLink = $"{_configuration["Frontend:TenantBaseUrl"]}/auth/set-password?token={token}",
                ButtonText = "Complete Your Setup"
            };

            await _emailService.SendEmailAsync(emailMessage);

            _logger.LogInformation("Reset Email sent to {SchoolName} with Id:({SchoolId})",
                school.SchoolName, school.PublicId);

            return Result.Success<string>("Reset password link sent successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resend reset password link for SchoolPublicId: {SchoolPublicId}", command.SchoolPublicId);
            return Result.Failure<string>(SchoolErrors.ErrorOccured());
        }
    }

}
