using System;

namespace Application.HelpRequest;

public class HelpRequestMessageDto
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string? Title { get; set; }
    public ICollection<string> Attachments { get; set; } = new List<string>();
}

