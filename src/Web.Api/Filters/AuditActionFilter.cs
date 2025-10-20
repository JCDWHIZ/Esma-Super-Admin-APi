using Application.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Api.Filters;

public class AuditActionFilter : IAsyncActionFilter
{
    private readonly string _actionDescription;
    private readonly IAuditLogService _auditLogService;
    private readonly ILogger<AuditActionFilter> _logger;

    public AuditActionFilter(
        string actionDescription,
        IAuditLogService auditLogService,
        ILogger<AuditActionFilter> logger)
    {
        _actionDescription = actionDescription;
        _auditLogService = auditLogService;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ActionExecutedContext executedContext = null;

        try
        {
            executedContext = await next();

            if (executedContext.Exception == null)
            {
                await _auditLogService.LogAsync(_actionDescription);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in audit filter for action: {Action}", _actionDescription);
            Console.WriteLine(ex);
        }
    }
}
