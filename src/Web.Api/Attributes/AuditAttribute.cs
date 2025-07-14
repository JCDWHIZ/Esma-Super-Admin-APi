using System;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Filters;

namespace Web.Api.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class AuditAttribute : TypeFilterAttribute
{
    public AuditAttribute(string actionDescription)
        : base(typeof(AuditActionFilter))
    {
        Arguments = new object[] { actionDescription };
    }
}