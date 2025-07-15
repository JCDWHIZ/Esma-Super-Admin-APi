using System;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Application.Abstractions.Models;

namespace admin_service.Web.Middleware;

public class ApiResponseMiddleware
{
    private readonly RequestDelegate _next;

    public ApiResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        Stream originalBodyStream = context.Response.Body;
        using var memStream = new MemoryStream();
        context.Response.Body = memStream;
        await _next(context);
        if (context.Request.Path.StartsWithSegments("/swagger") ||
            context.Request.Path.StartsWithSegments("/api-docs") ||
            context.Response.StatusCode == StatusCodes.Status204NoContent ||
            context.Response.ContentType != null &&
                !context.Response.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
        {
            memStream.Seek(0, SeekOrigin.Begin);
            await memStream.CopyToAsync(originalBodyStream);
            return;
        }
        string customMessage = "Success";
        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            customMessage = "Authentication required. Please log in to access this resource.";
        }
        else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
        {
            Endpoint? endpoint = context.GetEndpoint();
            IReadOnlyList<AuthorizeAttribute>? authorizeAttributes = endpoint?.Metadata.GetOrderedMetadata<AuthorizeAttribute>();

            string policyInfo = "";
            if (authorizeAttributes != null && authorizeAttributes.Any())
            {
                IEnumerable<string?> policies = authorizeAttributes
                    .Where(a => !string.IsNullOrEmpty(a.Policy))
                    .Select(a => a.Policy);

                if (policies.Any())
                {
                    policyInfo = $" Required policies: {string.Join(", ", policies)}.";
                }
            }

            customMessage = $"Access denied. You don't have the required permissions to access this resource.{policyInfo}";
        }
        else if (context.Response.StatusCode >= 400)
        {
            customMessage = "Error";
        }
        memStream.Seek(0, SeekOrigin.Begin);
        string responseBodyText = await new StreamReader(memStream).ReadToEndAsync();

        object? dataObject;
        try
        {
            dataObject = JsonSerializer.Deserialize<object>(responseBodyText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (dataObject is string innerJson)
            {
                object? temp = JsonSerializer.Deserialize<object>(innerJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (temp != null)
                {
                    dataObject = temp;
                }
                ;
            }
        }
        catch
        {
            dataObject = responseBodyText;
        }
        if (string.IsNullOrWhiteSpace(responseBodyText) && context.Response.StatusCode >= 400)
        {
            dataObject = null;
        }
        var apiResponse = new ApiResponse<object>
        {
            Status = context.Response.StatusCode,
            Data = dataObject,
            Message = customMessage,
            Timestamp = DateTime.UtcNow
        };
        context.Response.Body = originalBodyStream;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(apiResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}