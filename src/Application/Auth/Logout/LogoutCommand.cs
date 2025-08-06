using System;

namespace Application.Auth.Logout;

public sealed record LogoutCommand(string RefreshToken) : ICommand<bool>;