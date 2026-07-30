namespace Application.School;

public record LmsModuleResponseDto
{
    public string Name { get; init; } = string.Empty;
    public string Key { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

public record LmsModuleAvailabilityDto : LmsModuleResponseDto
{
    public bool HasModule { get; init; }
}
