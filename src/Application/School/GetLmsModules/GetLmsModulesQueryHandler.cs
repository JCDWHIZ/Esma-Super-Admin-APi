namespace Application.School.GetLmsModules;

public sealed class GetLmsModulesQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetLmsModulesQuery, IReadOnlyList<LmsModuleResponseDto>>
{
    public async Task<Result<IReadOnlyList<LmsModuleResponseDto>>> Handle(
        GetLmsModulesQuery query,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<LmsModuleResponseDto> modules = await context.LmsModules
            .OrderBy(m => m.Name)
            .Select(m => new LmsModuleResponseDto
            {
                Name = m.Name,
                Key = m.Key,
                Description = m.Description
            })
            .ToListAsync(cancellationToken);

        return Result.Success(modules);
    }
}
