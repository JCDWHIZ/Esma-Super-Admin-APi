using System;

namespace Application.Abstractions.Models;

public class TenantCreatedResponse
{
    public required string SchoolId { get; set; }
    public required string TenantId { get; set; } // add this to school table to be saved
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}