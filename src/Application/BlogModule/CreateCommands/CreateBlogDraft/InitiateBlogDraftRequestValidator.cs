using System;
using admin_service.Domain.Enums;

namespace admin_service.Application.BlogModule.Commands.CreateBlog;

public class InitiateBlogDraftRequestValidator : AbstractValidator<IntiateBlogDraftRequestCommand>
{
    public InitiateBlogDraftRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.");
        
        RuleFor(x => x.BackdropUrl)
            .Matches(@"^(http|https)://").When(x => !string.IsNullOrEmpty(x.BackdropUrl))
            .WithMessage("Backdrop URL must be a valid URL starting with http or https.");

        RuleFor(x => x.Status)
            .Must(status => Enum.IsDefined(typeof(BlogStatus), status))
            .WithMessage("Status must be a valid enum value.");

    }
}
