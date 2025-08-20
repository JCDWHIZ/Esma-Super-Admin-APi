using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Services;
using Domain.Roles;

namespace Application.Roles;
internal sealed class SyncRolesDomainEventHandler(KeycloakRolesService _keycloakRolesService) : IDomainEventHandler<SyncRolesDomainEvent>
{
    public async Task Handle(SyncRolesDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await _keycloakRolesService.SyncRolesToKeycloakAsync();
    }
}
