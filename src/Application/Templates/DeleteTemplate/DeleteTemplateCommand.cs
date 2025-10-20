namespace Application.Templates.DeleteTemplate;
public sealed record DeleteTemplateCommand(Guid PublicId) : ICommand<string>;
