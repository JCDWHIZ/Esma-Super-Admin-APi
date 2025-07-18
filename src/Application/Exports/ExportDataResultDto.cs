using System;

namespace Application.Exports;

public class ExportDataResultDto
{
    public required byte[] FileBytes { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
}
