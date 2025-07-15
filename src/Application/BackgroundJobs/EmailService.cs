using System;
using admin_service.Application.Common.Interfaces;
using Application.Abstractions.Models;

namespace admin_service.Application.Common.BackgroundJobs;

public class EmailService : IEmailService
{
    private readonly IMessageProducer _messageProducer;
    private const string EmailTopic = "persistent://public/default/email-actions";

    public EmailService(IMessageProducer messageProducer)
    {
        _messageProducer = messageProducer;
    }

    public async Task<string> SendEmailAsync(EmailMessage emailMessage)
    {
        return await _messageProducer.SendMessageAsync("SendEmail", emailMessage, EmailTopic);
    }
}