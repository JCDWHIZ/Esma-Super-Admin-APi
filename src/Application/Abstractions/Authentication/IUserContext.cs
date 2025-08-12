namespace Application.Abstractions.Authentication;

public interface IUserContext
{
    Guid? UserPublicId { get; }
    string? UserRole { get; }
}
