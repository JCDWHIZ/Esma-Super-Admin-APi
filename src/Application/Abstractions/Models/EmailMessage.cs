namespace Application.Abstractions.Models;

public class EmailMessage
{
    public required string Email { get; set; }
    public required string Title { get; set; }
    public required string Name { get; set; }
    public string? ButtonLink { get; set; }
    public string? ButtonText { get; set; }
    public string? Description { get; set; }
    public bool EmailButton { get; set; }
}
