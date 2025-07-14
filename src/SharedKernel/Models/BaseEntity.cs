using System.ComponentModel.DataAnnotations.Schema;

namespace SharedKernel.Models;

public abstract class BaseEntity
{
    private readonly List<DomainEvent> _domainEvents = new();
    public int Id { get; }

    public Guid PublicId { get; private set; }

    [NotMapped] public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    public void Raise(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private static Guid GeneratePublicId()
    {
        return Guid.CreateVersion7();
    }

    protected BaseEntity()
    {
        PublicId = GeneratePublicId();
    }
}
