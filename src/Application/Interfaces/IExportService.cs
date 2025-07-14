using System;
using admin_service.Application.Exports.Queries;
using admin_service.Domain.Enums;

namespace admin_service.Application.Common.Interfaces;

 public interface IExportService
    {
        Task<ExportDataResultDto> ExportDataAsync(AdminModule module, ExportType exportType);
    }