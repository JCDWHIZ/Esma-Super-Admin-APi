using System;
using System.Text;
using System.Text.Json;
using admin_service.Application.Common.Interfaces;
using admin_service.Application.Common.Models;

namespace admin_service.Infrastructure.Services;

public class EmailServices(HttpClient httpClient) : IEmailService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task SendEmailAsync(string recipient, string subject, string body)
    {
        var payload = new
        {
            to = recipient,
            subject = subject,
            body = body
        };

        string json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _httpClient.PostAsync("https://external-email-api.com/send", content);
        response.EnsureSuccessStatusCode();
    }

    public Task<string> SendEmailAsync(EmailMessage emailMessage)
    {
        throw new NotImplementedException();
    }
}
