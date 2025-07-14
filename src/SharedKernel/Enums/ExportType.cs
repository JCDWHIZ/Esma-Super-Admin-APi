using System;

using System.Text.Json.Serialization;
namespace SharedKernel.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExportType
{
    CSV,
    PDF,
}
