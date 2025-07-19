using System;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Application.Abstractions.Models;

namespace Web.Api.Middleware;

public class ApiResponseMiddleware
{
    private readonly RequestDelegate _next;

    // Cache JsonSerializerOptions instances to avoid creating new ones for each request
    private static readonly JsonSerializerOptions DeserializeOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly JsonSerializerOptions SerializeOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

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

        // Properly dispose StreamReader to fix CA2000 warning
        string responseBodyText;
        using (var streamReader = new StreamReader(memStream))
        {
            responseBodyText = await streamReader.ReadToEndAsync();
        }

        object? dataObject;
        try
        {
            // Use cached JsonSerializerOptions instance
            dataObject = JsonSerializer.Deserialize<object>(responseBodyText, DeserializeOptions);
            if (dataObject is string innerJson)
            {
                object? temp = JsonSerializer.Deserialize<object>(innerJson, DeserializeOptions);
                if (temp != null)
                {
                    dataObject = temp;
                }
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

        // Use cached JsonSerializerOptions instance
        await context.Response.WriteAsJsonAsync(apiResponse, SerializeOptions);
    }
}