namespace Application.Auth.ChangePassword;
public sealed record ChangePasswordCommand(string NewPassword) : ICommand<string>;
