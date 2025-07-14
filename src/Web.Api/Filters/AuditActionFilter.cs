using System;
using admin_service.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Api.Filters;

public class AuditActionFilter : IAsyncActionFilter
{
    private readonly string _actionDescription;
    private readonly IAuditLogService _auditLogService;

    public AuditActionFilter(string actionDescription, IAuditLogService auditLogService)
    {
        _actionDescription = actionDescription;
        _auditLogService = auditLogService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Execute the action first.
        await next();

        // Then log the audit event.
        await _auditLogService.LogAsync(_actionDescription);
    }
}
