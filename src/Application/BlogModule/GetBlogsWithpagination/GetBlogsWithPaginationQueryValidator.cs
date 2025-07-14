using System;

namespace admin_service.Application.BlogModule.Queries.GetBlogsWithpagination;

public class GetBlogsWithPaginationQueryValidator : AbstractValidator<GetBlogWithPaginationQuery>
{
    public GetBlogsWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0.");

        RuleFor(x => x.PublishDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .When(x => x.PublishDate.HasValue)
            .WithMessage("PublishDate cannot be in the future.");
    }
}