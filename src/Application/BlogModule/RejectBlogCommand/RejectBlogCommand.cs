using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.BlogModule.RejectBlogCommand;
public sealed record RejectBlogCommand(Guid PublicId, string RejectReason) : ICommand<string>;
