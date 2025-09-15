using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Roles;
using SharedKernel.Enums;
using SharedKernel.Models;

namespace Domain.Templates;

public class Template : BaseAuditableEntity
{
    public string TemplateName { get; set; }
    public string TemplateBody { get; set; }
    public TriggerType TemplateTrigger { get; set; }

    public static Template Create(string TemplateName, string TemplateBody, TriggerType TemplateTrigger) =>
        new() { TemplateName = TemplateName, TemplateBody = TemplateBody, TemplateTrigger = TemplateTrigger };
}
