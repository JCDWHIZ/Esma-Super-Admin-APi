using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Templates.UpdateTemplateCommand;
public sealed record UpdateTemplateCommand(Guid PublicId, string TemplateName, string TemplateBody, TriggerType TemplateTrigger) : ICommand<string>;
