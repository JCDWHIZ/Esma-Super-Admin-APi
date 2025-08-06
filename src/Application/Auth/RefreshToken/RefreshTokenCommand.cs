using System;
using Application.Interfaces;

namespace Application.Auth.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<RefreshTokenResponseDto>;