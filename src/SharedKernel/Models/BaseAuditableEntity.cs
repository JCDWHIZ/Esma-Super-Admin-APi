namespace SharedKernel.Models;

public abstract class BaseAuditableEntity : BaseEntity, ISoftDelete
{
    public DateTimeOffset Created { get; set; }

    public Guid? CreatedBy { get; set; }

    public Guid? LastModifiedBy { get; set; }
    public DateTimeOffset LastModified { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
