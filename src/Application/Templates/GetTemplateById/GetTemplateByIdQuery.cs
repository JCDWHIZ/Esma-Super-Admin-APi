namespace Application.Templates.GetTemplateById;
public sealed record GetTemplateByIdQuery(Guid PublicId) : IQuery<TemplateDto>;
