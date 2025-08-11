using System;

namespace Application.Roles.DeleteRole;

public sealed record DeleteRoleCommand(Guid PublicId) : ICommand<string>;