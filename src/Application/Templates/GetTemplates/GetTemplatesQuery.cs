using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Templates.GetTemplates;
public sealed record GetTemplatesWithPaginationQuery(int? Page, int? PageSize) : IQuery<PaginatedList<TemplateDto>>;
