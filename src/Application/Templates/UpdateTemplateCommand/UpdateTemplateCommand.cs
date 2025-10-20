namespace Application.Templates.UpdateTemplateCommand;
public sealed record UpdateTemplateCommand(Guid PublicId, string TemplateName, string TemplateBody) : ICommand<string>;
