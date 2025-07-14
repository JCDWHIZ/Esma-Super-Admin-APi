
    using System;
using admin_service.Domain.Entities;
using admin_service.Domain.Enums;

namespace admin_service.Application.HelpRequest.Queries;

public class HelpRequestItemDto
{
    public int Id { get; set; }
    public string? TicketId { get; set; }
    public HelpStatus Status { get; set; } = HelpStatus.OPEN_REQUEST;
    public HelpCategory Category { get; set; }
    public string PublicId { get; set; } = string.Empty;
    public ICollection<HelpRequestMessages> Messages { get; set; } = new List<HelpRequestMessages>();


    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<HelpRequests, HelpRequestItemDto>();
        }
    }

}
