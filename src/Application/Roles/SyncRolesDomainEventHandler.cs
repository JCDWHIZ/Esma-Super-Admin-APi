using Application.Interfaces;
using Domain.Roles;

namespace Application.Roles;
internal sealed class SyncRolesDomainEventHandler(IKeycloakRolesService _keycloakRolesService) : IDomainEventHandler<SyncRolesDomainEvent>
{
    public async Task Handle(SyncRolesDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await _keycloakRolesService.SyncRolesToKeycloakAsync();
    }
}

