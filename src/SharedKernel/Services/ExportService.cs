using System;
using admin_service.Application.Common.Interfaces;
using admin_service.Application.Exports;
using admin_service.Application.Exports.Queries;
using admin_service.Domain.Enums;
namespace admin_service.Infrastructure.Services;

public class ExportService : IExportService

    {
        private readonly Dictionary<AdminModule, IExportStrategy> _strategies;

        public ExportService(IEnumerable<IExportStrategy> exportStrategies)
        {
            _strategies = exportStrategies.ToDictionary(s => s.ModuleType);
        }

        public async Task<ExportDataResultDto> ExportDataAsync(AdminModule module, ExportType exportType)
        {
            if (!_strategies.TryGetValue(module, out var strategy))
            {
                throw new ArgumentException($"No export strategy found for module '{module}'.");
            }

            return await strategy.ExportDataAsync(exportType);
        }
    }