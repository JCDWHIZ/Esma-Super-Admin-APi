using System;
using System.Text;
using System.Text.Json;
using Application.Interfaces;
using Application.Abstractions.Models;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

// public class EmailServices(HttpClient httpClient) : IEmailService
// {
//     private readonly HttpClient _httpClient = httpClient;

//     public async Task SendEmailAsync(string recipient, string subject, string body)
//     {
//         var payload = new
//         {
//             to = recipient,
//             subject = subject,
//             body = body
//         };

//         string json = JsonSerializer.Serialize(payload);
//         var content = new StringContent(json, Encoding.UTF8, "application/json");
//         HttpResponseMessage response = await _httpClient.PostAsync("https://external-email-api.com/send", content);
//         response.EnsureSuccessStatusCode();
//     }

//     public Task<string> SendEmailAsync(EmailMessage emailMessage)
//     {
//          var payload = new
//         {
//             to = recipient,
//             subject = subject,
//             body = body
//         };

//         string json = JsonSerializer.Serialize(payload);
//         var content = new StringContent(json, Encoding.UTF8, "application/json");
//         HttpResponseMessage response = await _httpClient.PostAsync("https://external-email-api.com/send", content);
//         response.EnsureSuccessStatusCode();
//     }
// }
public class EmailService : IEmailService
{
    private readonly IMessageProducer _messageProducer;
    private readonly KafkaSettings _kafkaSettings;

    public EmailService(IMessageProducer messageProducer, IOptions<KafkaSettings> kafkaSettings)
    {
        _messageProducer = messageProducer;
        _kafkaSettings = kafkaSettings.Value;
    }

    public async Task<string> SendEmailAsync(EmailMessage emailMessage)
    {
        return await _messageProducer.SendMessageAsync("SendEmail", emailMessage, _kafkaSettings.EmailTopic);
    }
}