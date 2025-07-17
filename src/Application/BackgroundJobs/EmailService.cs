using System;
using Application.Abstractions.Models;
using Application.Interfaces;

namespace Application.BackgroundJobs;

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