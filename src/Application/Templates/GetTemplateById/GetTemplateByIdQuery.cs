using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Templates.GetTemplateById;
public sealed record GetTemplateByIdQuery(Guid PublicId) : IQuery<TemplateDto>;
