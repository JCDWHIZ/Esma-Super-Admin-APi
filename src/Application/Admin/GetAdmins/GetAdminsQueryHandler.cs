using System;
using Application.Interfaces;
using Application.Abstractions.Models;
using IApplicationDbContext = Application.Abstractions.Data.IApplicationDbContext;

namespace Application.Admin.GetAdmins;

public class GetAdminsQueryHandler(IApplicationDbContext _context) : IQueryHandler<GetAdminsQuery, PaginatedList<UserDto>>
{
    public async Task<Result<PaginatedList<UserDto>>> Handle(GetAdminsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<User> userQuery = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(query.Username))
        {
            userQuery = userQuery.Where(x => x.Username.Contains(query.Username));
        }
        if (query.Role.HasValue)
        {
            userQuery = userQuery.Where(x => x.Role == query.Role.Value);
        }

        PaginatedList<UserDto> pagedAdmins = await PaginatedList<UserDto>.CreateAsync(
            userQuery.Select(r => new UserDto
            {
                Id = r.Id,
                Email = r.Email,
                Role = r.Role,
                Username = r.Username,
                FirstName = r.FirstName,
                LastName = r.LastName,
                ProfilePic = r.ProfilePic
            }),
            query.PageNumber ?? 1,
            query.PageSize ?? 10
        );

        return Result.Success(pagedAdmins);
    }
}