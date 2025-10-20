using Application.Interfaces;

namespace Application.Exports;

public record ExportDataCommand(AdminModule Module, ExportType ExportType) : ICommand<ExportDataResultDto>;


public sealed class ExportDataCommandHandler : ICommandHandler<ExportDataCommand, ExportDataResultDto>
{
    private readonly IExportService _exportService;

    public ExportDataCommandHandler(IExportService exportService)
    {
        _exportService = exportService;
    }

    async Task<Result<ExportDataResultDto>> ICommandHandler<ExportDataCommand, ExportDataResultDto>.Handle(ExportDataCommand command, CancellationToken cancellationToken)
    {
        return await _exportService.ExportDataAsync(command.Module, command.ExportType);
    }
}
