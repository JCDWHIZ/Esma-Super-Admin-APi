using System;
using Application.School;
using Application.School.GetSchoolById;
using Infrastructure.Authorization;

namespace Web.Api.Endpoints.School;

public class GetSchoolById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("schools/{publicId:guid}", async (
            Guid publicId,
            IQueryHandler<GetSchoolByIdQuery, SchoolItemDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetSchoolByIdQuery(publicId);

            Result<SchoolItemDto> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Schools)
        .Produces<SchoolItemDto>(StatusCodes.Status200OK)
        .RequireAuthorization(new RequirePermissionAttribute("ViewSchool"));
    }
}
