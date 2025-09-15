using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Templates;

namespace Application.Templates.GetTemplateById;

public sealed class GetTemplateByIdQueryHandler(IApplicationDbContext context) : IQueryHandler<GetTemplateByIdQuery, TemplateDto>
{
    public async Task<Result<TemplateDto>> Handle(GetTemplateByIdQuery query, CancellationToken cancellationToken)
    {
        TemplateDto? template = await context.Templates
            .AsNoTracking()
            .Where(t => t.PublicId == query.PublicId)
            .Select(t => new TemplateDto
            {
                PublicId = t.PublicId,
                TemplateName = t.TemplateName,
                TemplateBody = t.TemplateBody,
                TemplateTrigger = t.TemplateTrigger,
                LastModified = t.LastModified
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (template == null)
        {
            return Result.Failure<TemplateDto>(TemplateErrors.NotFound(query.PublicId));
        }

        return Result.Success(template);
    }
}
