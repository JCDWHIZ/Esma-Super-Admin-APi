using System;

namespace admin_service.Application.Exports.Queries;

public class ExportDataResultDto
{
    public required byte[] FileBytes { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
}
