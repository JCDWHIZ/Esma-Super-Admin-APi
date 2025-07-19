
using Application.Interfaces;

namespace Application.School.GetSchoolStats;

public record GetSchoolStatsQuery : IQuery<SchoolStatsCountDto>;

public sealed class GetSchoolStatsQueryHandler(IApplicationDbContext _context) : IQueryHandler<GetSchoolStatsQuery, SchoolStatsCountDto>
{
    async Task<Result<SchoolStatsCountDto>> IQueryHandler<GetSchoolStatsQuery, SchoolStatsCountDto>.Handle(GetSchoolStatsQuery query, CancellationToken cancellationToken)
    {
        SchoolStatsCountDto? schoolStats = await _context.Schools
            .GroupBy(_ => true)
            .Select(g => new SchoolStatsCountDto
            {
                TotalSchools = g.Count(),
                SubscribedSchools = g.Count(x => x.Subscribed),
                UnsubscribedSchools = g.Count(x => !x.Subscribed)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return schoolStats ?? new SchoolStatsCountDto();
    }
}

public class SchoolStatsCountDto
{
    public int TotalSchools { get; set; }
    public int SubscribedSchools { get; set; }
    public int UnsubscribedSchools { get; set; }
}