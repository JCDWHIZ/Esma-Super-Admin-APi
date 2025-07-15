using System;

namespace Application.Abstractions.Models;

public class ApiResponse<T>
{
    /// <summary>
    /// The timestamp when the response is created.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The payload of the response.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Returns true if Status is between 200 and 299.
    /// </summary>
    public bool IsSuccess => Status >= 200 && Status < 300;

    /// <summary>
    /// Returns true if the response is not successful.
    /// </summary>
    public bool IsError => !IsSuccess;

    /// <summary>
    /// The HTTP status code.
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Optional message to accompany the response.
    /// </summary>
    public string? Message { get; set; }

    public ApiResponse() { }

    public ApiResponse(int status, T data, string? message = null)
    {
        Data = data;
        Status = status;
        Message = message;
    }

    public ApiResponse(int status, string? message = null)
    {
        Status = status;
        Message = message;
    }
}