using Application.Exports;

namespace Application.Interfaces;

public interface IExportService
{
    Task<ExportDataResultDto> ExportDataAsync(AdminModule module, ExportType exportType);
}