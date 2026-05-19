using Domain.Schools;

namespace Application.School.GetSchoolModules;

public sealed class GetSchoolModulesQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetSchoolModulesQuery, IReadOnlyList<SchoolModuleResponseDto>>
{
    public async Task<Result<IReadOnlyList<SchoolModuleResponseDto>>> Handle(
        GetSchoolModulesQuery query,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<SchoolModuleResponseDto> modules = await context.SchoolModules
            .OrderBy(m => m.Name)
            .Select(m => new SchoolModuleResponseDto
            {
                Name = m.Name,
                Key = m.Key,
                Description = m.Description
            })
            .ToListAsync(cancellationToken);

        return Result.Success(modules);
    }
}
