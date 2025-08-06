using System;

namespace Application.Auth.ForgotPassword;

public class ForgotPasswordResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}