using System;
using admin_service.Domain.Entities;
using admin_service.Domain.Enums;

namespace admin_service.Application.Admin.Queries;

public class UserDto
{
    public Roles Role { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName {get; set;} = string.Empty;
    public string PhoneNumber {get; set;} = string.Empty;
    public string LastName {get; set;} = string.Empty;
    public string PublicId { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    // public int Signups { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<User, UserDto>();
        }
    }
}
