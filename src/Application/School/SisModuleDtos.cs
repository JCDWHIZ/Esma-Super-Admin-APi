namespace Application.School;

public record SisModuleResponseDto
{
    public string Name { get; init; } = string.Empty;
    public string Key { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public record SisModuleAvailabilityDto : SisModuleResponseDto
{
    public bool HasModule { get; init; }
}
