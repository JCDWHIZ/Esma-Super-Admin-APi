using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Templates.DeleteTemplate;
public sealed record DeleteTemplateCommand(Guid PublicId) : ICommand<string>;
