using System;
using Application;
using Application.Admin;
using Application.Admin.GetAdmins;
using SharedKernel.Enums;

namespace Web.Api.Endpoints.Admin;

public sealed class GetAdmins : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/admin", async (
            int pageNumber,
            int pageSize,
            string username,
            Roles role,
            IQueryHandler<GetAdminsQuery, PaginatedList<UserDto>> handler,
            CancellationToken cancellationToken
        ) =>
        {
            var query = new GetAdminsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Username = username,
                Role = role
            };

            Result<PaginatedList<UserDto>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName("GetAdmins")
        .WithTags(Tags.Admin)
        .RequireAuthorization();
    }
}
