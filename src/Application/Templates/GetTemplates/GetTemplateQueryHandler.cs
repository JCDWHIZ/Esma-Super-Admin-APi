using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Templates;

namespace Application.Templates.GetTemplates;

public sealed class GetTemplatesWithPaginationQueryHandler(IApplicationDbContext context) : IQueryHandler<GetTemplatesWithPaginationQuery, PaginatedList<TemplateDto>>
{
    public async Task<Result<PaginatedList<TemplateDto>>> Handle(GetTemplatesWithPaginationQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Template> templatesQuery = context.Templates
            .AsNoTracking();

        IQueryable<TemplateDto> templateDtosQuery = templatesQuery.Select(t => new TemplateDto
        {
            PublicId = t.PublicId,
            TemplateName = t.TemplateName,
            LastModified = t.LastModified,
            TemplateTrigger = t.TemplateTrigger
        });

        PaginatedList<TemplateDto> paginatedTemplates = await PaginatedList<TemplateDto>.CreateAsync(
            templateDtosQuery,
            query.Page ?? 1,
            query.PageSize ?? 10
        );

        return Result.Success(paginatedTemplates);
    }
}
