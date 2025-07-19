// using System;
// using admin_service.Domain.Enums;

// namespace admin_service.Application.HelpRequest.Queries.GetHelpRequestWithPagination;

// public class GetHelpRequestWithPaginationQueryValidator: AbstractValidator<GetHelpRequestWithPaginationQuery>
// {
//     public GetHelpRequestWithPaginationQueryValidator()
//     {
//         RuleFor(x => x.TicketId)
//             .MaximumLength(100).WithMessage
//             ("TicketId must not exceed 100 characters.");

//         RuleFor(x => x.PageNumber)
//             .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

//         RuleFor(x => x.PageSize)
//             .GreaterThanOrEqualTo(1).WithMessage("Page size must be at least 1.")
//             .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100.");
//     }
// }
