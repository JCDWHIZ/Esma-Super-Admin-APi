namespace Application.Auth.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : ICommand<ForgotPasswordResponseDto>;
