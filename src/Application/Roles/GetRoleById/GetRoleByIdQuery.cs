namespace Application.Roles.GetRoleById;
public record GetRoleByIdQuery(Guid RolePublicId) : IQuery<RolesDto>;
