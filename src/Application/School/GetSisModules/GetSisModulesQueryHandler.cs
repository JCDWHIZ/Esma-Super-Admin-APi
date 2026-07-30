namespace Application.School.GetSisModules;

public sealed class GetSisModulesQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetSisModulesQuery, IReadOnlyList<SisModuleResponseDto>>
{
    public async Task<Result<IReadOnlyList<SisModuleResponseDto>>> Handle(
        GetSisModulesQuery query,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<SisModuleResponseDto> modules = await context.SisModules
            .OrderBy(m => m.Name)
            .Select(m => new SisModuleResponseDto
            {
                Name = m.Name,
                Key = m.Key,
                Description = m.Description
            })
            .ToListAsync(cancellationToken);

        return Result.Success(modules);
    }
}
