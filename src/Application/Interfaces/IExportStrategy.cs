using System;
using admin_service.Application.Exports.Queries;
using admin_service.Domain.Enums;

namespace admin_service.Application.Common.Interfaces;

// In Application/Common/Interfaces/IExportStrategy.cs
public interface IExportStrategy
{
    /// <summary>
    /// The module name this strategy handles (e.g., "AuditLogs", "Users", "Schools").
    /// </summary>
    AdminModule ModuleType { get; }
    
    /// <summary>
    /// Exports the data for the given module in the specified format.
    /// </summary>
    Task<ExportDataResultDto> ExportDataAsync(ExportType exportType);
}