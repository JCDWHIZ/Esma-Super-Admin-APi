namespace Application.Auth.SetPassword;

public sealed record SetPasswordCommand(string NewPassword, string AccessToken) : ICommand<string>;