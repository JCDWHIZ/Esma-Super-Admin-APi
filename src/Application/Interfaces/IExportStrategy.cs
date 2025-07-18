using System;
using Application.Exports;

namespace Application.Interfaces;

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