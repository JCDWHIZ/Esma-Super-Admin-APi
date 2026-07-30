using Application.School.CreateSchool;
using Application.School;

namespace Application.Abstractions.Models;

public class CreateTenantMessage
{
    public int SchoolId { get; set; }
    //public string OrganizationId { get; set; }
    public Guid SchoolPublicId { get; set; }
    public string SchoolName { get; set; }
    public string ShortCode { get; set; }
    public string SchoolAdminEmail { get; set; }
    public string SchoolAdminFirstName { get; set; }
    public string SchoolAdminLastName { get; set; }
    public string SchoolAdminUsername { get; set; }
    public string? SchoolAdminPhoneNumber { get; set; }
    public string SchoolLogoUrl { get; set; }
    public string SchoolEmail { get; set; }
    public string? SchoolPhoneNumber { get; set; }

    // Legacy, SIS-only module list
    public ICollection<string> Modules { get; set; } = new List<string>();

    // Per-product module lists
    public ICollection<string>? SisModules { get; set; }
    public ICollection<string>? LmsModules { get; set; }

    // Which product(s) to provision (optional)
    public ICollection<string> Products { get; set; } = new List<string>();

    // Optional LMS-specific fields (consulted only when LMS is requested)
    public string? LmsDeploymentMode { get; set; }
    public string? LmsSlug { get; set; }
    public string? LmsLearningDomainTemplateId { get; set; }

    public AddressDto? SchoolAddress { get; set; }
    public SharedKernel.Enums.Roles SchoolAdminRole { get; set; }
    public SubscriptionDto Subscriptions { get; set; }

    /// <summary>
    /// Resolves which products this message requests provisioned, in priority order:
    /// 1) explicit <see cref="Products"/> if non-empty (case-insensitive, accepts "SIS", "ESMA", "LMS")
    /// 2) fallback to Modules containing "LMS" => {SIS, LMS}
    /// 3) default => {SIS}
    /// </summary>
    /// <returns>Ordered list of resolved product types (duplicates removed).</returns>
    /// <exception cref="InvalidOperationException">When an unknown product value is supplied.</exception>
    public IList<ProductType> ResolveProducts()
    {
        if (Products != null && Products.Any())
        {
            var resolved = new List<ProductType>();
            foreach (string raw in Products)
            {
                if (string.IsNullOrWhiteSpace(raw))
                {
                    continue;
                }

                ProductType pt = ParseProduct(raw);
                if (!resolved.Contains(pt))
                {
                    resolved.Add(pt);
                }
            }

            return resolved;
        }

        if (Modules != null && Modules.Any(m => !string.IsNullOrWhiteSpace(m) && m.Trim().Equals("LMS", StringComparison.OrdinalIgnoreCase)))
        {
            return new List<ProductType> { ProductType.SIS, ProductType.LMS };
        }

        return new List<ProductType> { ProductType.SIS };
    }

    private static ProductType ParseProduct(string raw)
    {
        string normalized = raw.Trim().ToUpperInvariant();
        return normalized switch
        {
            "SIS" or "ESMA" => ProductType.SIS,
            "LMS" => ProductType.LMS,
            _ => throw new InvalidOperationException($"Unknown product '{raw}' in Products list. Valid values: SIS, ESMA, LMS."),
        };
    }

    /// <summary>
    /// SIS modules to activate: returns <see cref="SisModules"/> if non-empty, otherwise falls back to legacy <see cref="Modules"/>.
    /// Returns null when neither is set (meaning "leave modules alone").
    /// </summary>
    public IList<string>? ResolveSisModules()
    {
        if (SisModules != null && SisModules.Any())
        {
            return SisModules.ToList();
        }

        if (Modules != null && Modules.Any())
        {
            return Modules.ToList();
        }

        return null;
    }

    /// <summary>
    /// LMS modules to activate. No legacy fallback; returns null/empty when unset.
    /// </summary>
    public IList<string>? ResolveLmsModules()
    {
        if (LmsModules != null && LmsModules.Any())
        {
            return LmsModules.ToList();
        }

        return null;
    }
}

public class UpdateTenantPayload : CreateTenantMessage
{
    public string TenantId { get; set; } = string.Empty;
}
