using System;
using Application.Exports;
using Application.Interfaces;
using SharedKernel.Enums;

namespace Infrastructure.Services;

public class ExportService : IExportService

{
    private readonly Dictionary<AdminModule, IExportStrategy> _strategies;

    public ExportService(IEnumerable<IExportStrategy> exportStrategies)
    {
        _strategies = exportStrategies.ToDictionary(s => s.ModuleType);
    }

    public async Task<ExportDataResultDto> ExportDataAsync(AdminModule module, ExportType exportType)
    {
        if (!_strategies.TryGetValue(module, out IExportStrategy? strategy))
        {
            throw new ArgumentException($"No export strategy found for module '{module}'.");
        }

        return await strategy.ExportDataAsync(exportType);
    }
}