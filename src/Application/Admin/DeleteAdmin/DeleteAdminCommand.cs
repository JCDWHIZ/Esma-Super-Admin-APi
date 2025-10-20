namespace Application.Admin.DeleteAdmin;
public sealed record DeleteAdminCommand(Guid PublicId) : ICommand<string>;
