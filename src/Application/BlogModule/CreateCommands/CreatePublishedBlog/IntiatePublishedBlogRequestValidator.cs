using System;
using admin_service.Application.BlogModule.Commands.CreateBlog;
using admin_service.Domain.Enums;

namespace admin_service.Application.BlogModule.Commands.CreatePublishedBlog;

public class IntiatePublishedBlogRequestValidator : AbstractValidator<IntiatePublishedBlogRequestCommand>
{
    public IntiatePublishedBlogRequestValidator()
    {
        RuleFor(x => x.Status)
            .Must(status => Enum.IsDefined(typeof(BlogStatus), status))
            .WithMessage("Status must be a valid enum value.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required.");
    }

}
