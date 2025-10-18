using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Templates;
public sealed record TemplateDto
{
    public Guid PublicId { get; init; }
    public string TemplateName { get; init; }
    public string? TemplateBody { get; init; }
    public List<string> ExpectedVariables { get; init; } = new();
    public TriggerType TemplateTrigger { get; init; }
    public DateTimeOffset? LastModified { get; init; }
}
