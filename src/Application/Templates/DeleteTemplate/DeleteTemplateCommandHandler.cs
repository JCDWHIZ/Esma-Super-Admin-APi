using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Templates;

namespace Application.Templates.DeleteTemplate;
internal sealed class DeleteTemplateCommandHandler(IApplicationDbContext context) : ICommandHandler<DeleteTemplateCommand, string>
{
    public async Task<Result<string>> Handle(DeleteTemplateCommand command, CancellationToken cancellationToken)
    {
        Template? template = await context.Templates
            .FirstOrDefaultAsync(t => t.PublicId == command.PublicId, cancellationToken);

        if (template == null)
        {
            return Result.Failure<string>(TemplateErrors.NotFound(command.PublicId));
        }

        context.Templates.Remove(template);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Template deleted successfully");
    }
}
