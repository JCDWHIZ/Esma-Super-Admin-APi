using System;
using Application.Abstractions.Models;

namespace Application.Interfaces;

public interface IEmailService
{
    Task<string> SendEmailAsync(EmailMessage emailMessage);
}
