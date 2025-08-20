using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Templates.CreateTemplateCommand;
public sealed record CreateTemplateCommand (string TemplateBody, string TemplateName, TriggerType TemplateTrigger) : ICommand<string>;
