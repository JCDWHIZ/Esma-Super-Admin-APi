using System;

namespace Application.Abstractions.Models;

public class CreateTenantMessage
{
    public required int SchoolId { get; set; }
    public required string OrganizationId { get; set; }
    public required Guid SchoolPublicId { get; set; }
    public required string SchoolName { get; set; }
    public required string SchoolAdminEmail { get; set; }
    public required string SchoolAdminFirstName { get; set; }
    public required string SchoolAdminLastName { get; set; }
}