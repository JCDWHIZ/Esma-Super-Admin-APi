namespace Application.Templates.CreateTemplateCommand;
public sealed record CreateTemplateCommand(string TemplateBody, string TemplateName, TriggerType TemplateTrigger) : ICommand<string>;
