using System;
using Application.Exports;

namespace Web.Api.Endpoints.Exports;

public sealed class GetExport : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("exports", async (
            AdminModule module,
            ExportType exportType,
            ICommandHandler<ExportDataCommand, ExportDataResultDto> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ExportDataCommand(module, exportType);

            Result<ExportDataResultDto> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Exports)
        .RequireAuthorization();
    }
}
