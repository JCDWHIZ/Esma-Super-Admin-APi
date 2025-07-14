using System;

namespace admin_service.Application.AuditLogModule.Queries.GetAuditLogWithPagination;

public class GetAuditLogWithPaginationQueryValidator : AbstractValidator<GetAuditLogWithPaginationQuery>
{
    public GetAuditLogWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("PageNumber must be greater than 0.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("PageSize must be greater than 0.");
    }
}

