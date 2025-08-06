using System;
using Application.Interfaces;

namespace Application.Auth.ChangePassword;

public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword, string AccessToken) : ICommand<ChangePasswordResponseDto>;