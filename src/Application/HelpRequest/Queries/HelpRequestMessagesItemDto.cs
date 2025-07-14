using System;
using admin_service.Application.Common.Mappings;
using admin_service.Domain.Entities;

namespace admin_service.Application.HelpRequest.Queries;

public class HelpRequestMessageDto
{
    public int Id { get; set; }
    public string PublicId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public ICollection<string> Attachments { get; set; } = new List<string>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<HelpRequestMessages, HelpRequestMessageDto>();
        }
    }
}

