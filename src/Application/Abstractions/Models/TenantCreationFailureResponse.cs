using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Models;
public class TenantCreationFailureResponse
{
    public required string SchoolPublicId { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
