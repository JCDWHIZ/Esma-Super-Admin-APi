
using System;

namespace Application.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<LoginCommandResponseDto>;
