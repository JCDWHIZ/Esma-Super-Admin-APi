using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Application.Auth;
public class GetUserSessionsQueryHandler(ILogger<GetUserSessionsQueryHandler> _logger, KeycloakRolesService _keycloakService) : IQueryHandler<GetUserSessionQuery, GetSessionsCommandResponseDto>
{
    public async Task<Result<GetSessionsCommandResponseDto>> Handle(GetUserSessionQuery query, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Attempting to retrieve sessions for authenticated user");

            List<KeycloakSessionDto> keycloakResponse = await _keycloakService.GetUserSessionsAsync();

            var response = new GetSessionsCommandResponseDto(
                sessions: keycloakResponse,
                success: true
            );

            _logger.LogInformation("Successfully retrieved {TotalCount} sessions/devices for authenticated user",
                response.TotalCount);

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user sessions");
            return Result.Failure<GetSessionsCommandResponseDto>(UserErrors.ErrorOccured());
        }
    }
}
