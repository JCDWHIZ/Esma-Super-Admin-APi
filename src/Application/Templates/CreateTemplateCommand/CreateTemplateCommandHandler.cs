using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Templates;

namespace Application.Templates.CreateTemplateCommand;
internal sealed class CreateTemplateCommandHandler(IApplicationDbContext _context) : ICommandHandler<CreateTemplateCommand, string>
{
    public async Task<Result<string>> Handle(CreateTemplateCommand command, CancellationToken cancellationToken)
    {
        Domain.Templates.Template? existingTemplate = await _context.Templates.FirstOrDefaultAsync(t => t.TemplateName == command.TemplateName || t.TemplateTrigger == command.TemplateTrigger, cancellationToken);

        if (existingTemplate != null)
        {
            return Result.Failure<string>(TemplateErrors.AlreadyExists());
        }

        Template.Create(command.TemplateName, command.TemplateName, command.TemplateTrigger);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("Template Created Successfully");
    }
}
