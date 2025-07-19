// using System;
// using admin_service.Domain.Enums;

// namespace admin_service.Application.Subscription.Queries.GetSubscriptionStats;

// public class GetSubscriptionWithPaginationQueryValidator: AbstractValidator<GetSubscriptionWithPaginationQuery>
// {
//     public GetSubscriptionWithPaginationQueryValidator()
//     {   
//         RuleFor(x => x.StartDate)
//             .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Start date must not be in the future.");

//         RuleFor(x => x.EndDate)
//             .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be after the start date.");

//         RuleFor(x => x.PageNumber)
//             .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

//         RuleFor(x => x.PageSize)
//             .GreaterThanOrEqualTo(1).WithMessage("Page size must be at least 1.");
//     }

// }
