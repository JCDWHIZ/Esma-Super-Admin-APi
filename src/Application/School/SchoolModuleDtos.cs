namespace Application.School;

public record SchoolModuleResponseDto
{
    public string Name { get; init; } = string.Empty;
    public string Key { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public record SchoolModuleAvailabilityDto : SchoolModuleResponseDto
{
    public bool HasModule { get; init; }
}
