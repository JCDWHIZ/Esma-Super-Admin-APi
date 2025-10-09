using System;
using Application.Interfaces;
using Application.Abstractions.Models;
using IApplicationDbContext = Application.Abstractions.Data.IApplicationDbContext;

namespace Application.Admin.GetAdmins;

public class GetAdminsQueryHandler(IApplicationDbContext _context) : IQueryHandler<GetAdminsQuery, PaginatedList<UserDto>>
{
    public async Task<Result<PaginatedList<UserDto>>> Handle(GetAdminsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<User> userQuery = _context.Users
            .Include(u => u.Role) // Include Role to access Role.Name
            .AsQueryable();

        if (!string.IsNullOrEmpty(query.Username))
        {
            userQuery = userQuery.Where(x => x.Username.Contains(query.Username));
        }
        if (!string.IsNullOrEmpty(query.RoleName)) // Changed from Role enum to RoleName string
        {
            userQuery = userQuery.Where(x => x.Role.Name == query.RoleName);
        }

        PaginatedList<UserDto> pagedAdmins = await PaginatedList<UserDto>.CreateAsync(
            userQuery.Select(r => new UserDto
            {
                PublicId = r.PublicId,
                Email = r.Email,
                Username = r.Username,
                FirstName = r.FirstName,
                LastName = r.LastName,
                PhoneNumber = r.PhoneNumber,
                ProfilePic = r.ProfilePic,
                RoleName = r.Role!.Name,
                CreatedAt = r.Created,
                CreatedBy = r.CreatedBy
            }),
            query.PageNumber ?? 1,
            query.PageSize ?? 10
        );

        return Result.Success(pagedAdmins);
    }
}
