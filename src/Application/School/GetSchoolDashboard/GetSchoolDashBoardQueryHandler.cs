using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace admin_service.Application.School.Queries.GetSchoolStats;

public sealed record GetSchoolDashboardQuery : IQuery<List<YearlyOverviewDto>>;
public sealed class GetSchoolDashboardQueryHandler(IApplicationDbContext _context) : IQueryHandler<GetSchoolDashboardQuery, List<YearlyOverviewDto>>
{
    async Task<Result<List<YearlyOverviewDto>>> IQueryHandler<GetSchoolDashboardQuery, List<YearlyOverviewDto>>.Handle(GetSchoolDashboardQuery query, CancellationToken cancellationToken)
    {
        var yearlyOverview = await _context.Schools.GroupBy(s => new { s.Created.Year, s.Created.Month }).Select(g => new
        {
            g.Key.Year,
            g.Key.Month,
            Count = g.Count()
        })
        .ToListAsync(cancellationToken);


        var groupedYearlyOverview = yearlyOverview.GroupBy(x => x.Year).Select(g => new YearlyOverviewDto
        {
            Year = g.Key,
            MonthlyOverview = [.. Enumerable.Range(1, 12)
        .Select(month =>
        {
            var monthData = g.FirstOrDefault(x => x.Month == month);
            int count = monthData?.Count ?? 0;
            return $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)}: {count}";
        })]
        }).ToList();

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
