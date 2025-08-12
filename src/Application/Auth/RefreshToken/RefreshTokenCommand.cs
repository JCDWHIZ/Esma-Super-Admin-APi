using System;
using Application.Auth.Login;
using Application.Interfaces;

namespace Application.Auth.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<LoginCommandResponseDto>;
