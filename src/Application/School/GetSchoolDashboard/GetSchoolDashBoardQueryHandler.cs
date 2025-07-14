using admin_service.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace admin_service.Application.School.Queries.GetSchoolStats;

public record GetSchoolDashboardQuery : IRequest<List<YearlyOverviewDto>>;
public class GetSchoolDashboardQueryHandler : IRequestHandler<GetSchoolDashboardQuery, List<YearlyOverviewDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSchoolDashboardQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<YearlyOverviewDto>> Handle(GetSchoolDashboardQuery request, CancellationToken cancellationToken)
    {
        var yearlyOverview = await _context.Schools
    .GroupBy(s => new { s.Created.Year, s.Created.Month })
    .Select(g => new 
    {
        Year = g.Key.Year,
        Month = g.Key.Month,
        Count = g.Count()
    })
    .ToListAsync(cancellationToken);


var groupedYearlyOverview = yearlyOverview
    .GroupBy(x => x.Year)
    .Select(g => new YearlyOverviewDto
    {
        Year = g.Key,
        MonthlyOverview = [.. Enumerable.Range(1, 12)
            .Select(month => 
            {
                var monthData = g.FirstOrDefault(x => x.Month == month);
                var count = monthData?.Count ?? 0;
                return $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)}: {count}";
            })]
    })
    .ToList();

return groupedYearlyOverview;
    }
}


// public class GetSchoolDashBoardQuery
// {
//     private readonly IApplicationDbContext _context;

//     public GetSchoolDashBoardQuery(IApplicationDbContext context)
//     {
//         _context = context;
//     }

//     public async Task<List<YearlyOverviewDto>> GetSchoolStats(CancellationToken cancellationToken)
//     {
//         var yearlyOverview = await _context.Schools
//     .GroupBy(s => new { s.Created.Year, s.Created.Month })
//     .Select(g => new 
//     {
//         Year = g.Key.Year,
//         Month = g.Key.Month,
//         Count = g.Count()
//     })
//     .ToListAsync(cancellationToken);


// var groupedYearlyOverview = yearlyOverview
//     .GroupBy(x => x.Year)
//     .Select(g => new YearlyOverviewDto
//     {
//         Year = g.Key,
//         MonthlyOverview = Enumerable.Range(1, 12)
//             .Select(month => 
//             {
//                 var monthData = g.FirstOrDefault(x => x.Month == month);
//                 var count = monthData?.Count ?? 0;
//                 return $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)}: {count}";
//             })
//             .ToList()
//     })
//     .ToList();

// return groupedYearlyOverview;
//     }
// }

public class YearlyOverviewDto
{
    public int Year { get; set; }
    public List<string> MonthlyOverview { get; set; } = new();
}
