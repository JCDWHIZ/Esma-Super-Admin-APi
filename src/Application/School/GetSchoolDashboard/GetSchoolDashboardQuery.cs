using System;

namespace Application.School.GetSchoolDashboard;

public sealed record GetSchoolDashboardQuery : IQuery<List<YearlyOverviewDto>>;