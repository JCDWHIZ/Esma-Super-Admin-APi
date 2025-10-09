using System;

namespace Application.Admin.GetAdmins;

public sealed record GetAdminsQuery : IQuery<PaginatedList<UserDto>>
{
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
    public string? Username { get; set; }
    public string? RoleName { get; set; }
}
