using System;

namespace Application.Abstractions.Models;

public class CreateTenantMessage
{
    public required string SchoolId { get; set; }
    public required string OrganizationId { get; set; }
    public required string SchoolPublicId { get; set; }
    public required string SchoolName { get; set; }
    public required string SchoolAdminEmail { get; set; }
    public required string SchoolAdminFirstName { get; set; }
    public required string SchoolAdminLastName { get; set; }
}


// public class TenantCreatedResponse
// {
//     public required string SchoolId { get; set; }
//     public required string TenantId { get; set; }
//     public bool Success { get; set; }
//     public string? ErrorMessage { get; set; }
//     public required CreateSchoolCommand OriginalCommand { get; set; }
// }