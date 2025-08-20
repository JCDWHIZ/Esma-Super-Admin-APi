using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Admin.DeleteAdmin;
public sealed record DeleteAdminCommand(Guid PublicId) : ICommand<string>;
