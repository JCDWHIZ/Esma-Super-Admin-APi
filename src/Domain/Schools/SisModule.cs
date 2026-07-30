using SharedKernel.Models;

namespace Domain.Schools;

public sealed class SisModule : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<Schools> Schools { get; set; } = new List<Schools>();
}
