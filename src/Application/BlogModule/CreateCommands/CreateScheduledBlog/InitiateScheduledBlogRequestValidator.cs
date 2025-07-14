using System;
using admin_service.Application.BlogModule.Commands.CreateBlog;
using admin_service.Domain.Enums;

namespace admin_service.Application.BlogModule.Commands.CreateScheduledBlog;

public class InitiateScheduledBlogRequestValidator : AbstractValidator<InitiateScheduledBlogRequestCommand>
{
    public InitiateScheduledBlogRequestValidator()
    {
        RuleFor(x => x.PublishDate)
            .NotEmpty().WithMessage("Publish date is required.")
            .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("Publish date must be in the future.");

        RuleFor(x => x.Status)
            .Must(status => Enum.IsDefined(typeof(BlogStatus), status))
            .WithMessage("Status must be a valid enum value.");
    }

}
