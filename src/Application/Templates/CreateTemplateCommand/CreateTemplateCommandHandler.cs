using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Templates;

namespace Application.Templates.CreateTemplateCommand;

public sealed class CreateTemplateCommandHandler(IApplicationDbContext _context) : ICommandHandler<CreateTemplateCommand, string>
{
    public async Task<Result<string>> Handle(CreateTemplateCommand command, CancellationToken cancellationToken)
    {
        Template? existingTemplate = await _context.Templates.FirstOrDefaultAsync(t => t.TemplateName == command.TemplateName || t.TemplateTrigger == command.TemplateTrigger, cancellationToken);

        if (existingTemplate != null)
        {
            return Result.Failure<string>(TemplateErrors.AlreadyExists());
        }

        var template = new Template
        {
            TemplateBody = command.TemplateBody,
            TemplateName = command.TemplateName,
            TemplateTrigger = command.TemplateTrigger
        };

        _context.Templates.Add(template);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("Template Created Successfully");
    }
}
