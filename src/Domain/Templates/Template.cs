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
    public List<string> ExpectedVariables { get; set; } = new();

    public static Template Create(string TemplateName, string TemplateBody, TriggerType TemplateTrigger, List<string> ExpectedVariables) =>
        new() { TemplateName = TemplateName, TemplateBody = TemplateBody, TemplateTrigger = TemplateTrigger };

    public string RenderTemplate(Template template, Dictionary<string, string> variables)
    {
        string content = template.TemplateBody;

        foreach (string variable in template.ExpectedVariables)
        {
            content = variables.TryGetValue(variable, out string? value)
                ? content.Replace($"*{variable}*", value)
                : content.Replace($"*{variable}*", $"[{variable} not provided]");
        }

        return content;
    }
}
