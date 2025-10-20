namespace Application.Templates.GetTemplates;
public sealed record GetTemplatesWithPaginationQuery(int? Page, int? PageSize) : IQuery<PaginatedList<TemplateDto>>;
