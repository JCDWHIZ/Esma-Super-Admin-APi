using System;
using admin_service.Domain.Entities;

namespace admin_service.Application.AuditLogModule.Queries;

public class AuditLogDto
{
    public string? Role { get; set; } = string.Empty;
    public string? Action { get; set; } = string.Empty;
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<AuditLog, AuditLogDto>();
        }
    }
}
