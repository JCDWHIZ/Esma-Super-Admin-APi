using Domain.Templates;

namespace Application.Templates.UpdateTemplateCommand;

public sealed class UpdateTemplateCommandHandler(IApplicationDbContext context) : ICommandHandler<UpdateTemplateCommand, string>
{
    public async Task<Result<string>> Handle(UpdateTemplateCommand command, CancellationToken cancellationToken)
    {
        Template? template = await context.Templates
            .FirstOrDefaultAsync(t => t.PublicId == command.PublicId, cancellationToken);

        if (template == null)
        {
            return Result.Failure<string>(TemplateErrors.NotFound(command.PublicId));
        }

        //Template? existingTemplate = await context.Templates
        //    .FirstOrDefaultAsync(t => (t.TemplateName == command.TemplateName || t.TemplateTrigger == command.TemplateTrigger)
        //        && t.PublicId != command.PublicId, cancellationToken);

        //if (existingTemplate != null)
        //{
        //    return Result.Failure<string>(TemplateErrors.AlreadyExists());
        //}

        template.TemplateName = command.TemplateName;
        template.TemplateBody = command.TemplateBody;
        //template.TemplateTrigger = command.TemplateTrigger;

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success("Template updated successfully");
    }
}
