using System;
using admin_service.Domain.Entities;
using admin_service.Domain.Enums;

public class BlogItemDto
{
    public string PublicId { get; set; } = string.Empty;
    public int Id { get; set; }
    public string BackdropUrl {get; set;} = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public BlogStatus Status { get; set; } = BlogStatus.DRAFT;
    public DateTime? PublishDate { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Blog, BlogItemDto>();
        }
    }
}
