
using admin_service.Application.Common.Interfaces;


namespace admin_service.Application.School.Queries.GetSchoolStats;

public record GetSchoolStatsQuery : ICommand<SchoolStatsCountDto>;

public class GetSchoolStatsQueryHandler : ICommandHandler<GetSchoolStatsQuery, SchoolStatsCountDto>
{
    private readonly IApplicationDbContext _context;

    public GetSchoolStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SchoolStatsCountDto> Handle(GetSchoolStatsQuery request, CancellationToken cancellationToken)
    {
        var schoolStats = await _context.Schools
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