using System;
using admin_service.Application.Common.Models;

namespace admin_service.Application.Common.Interfaces;

public interface IEmailService
{
    Task<string> SendEmailAsync(EmailMessage emailMessage);
}
