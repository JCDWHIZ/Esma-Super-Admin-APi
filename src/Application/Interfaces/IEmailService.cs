using System;
using Application.Abstractions.Models;

namespace admin_service.Application.Common.Interfaces;

public interface IEmailService
{
    Task<string> SendEmailAsync(EmailMessage emailMessage);
}
