using System;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Web.Api.Filters;

namespace Web.Api.Attributes;

//[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
//public sealed class AuditAttribute : TypeFilterAttribute
//{
//    public AuditAttribute(string actionDescription) : base(typeof(AuditActionFilter))
//    {
//        Arguments = [actionDescription];
//    }
//}
//[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
//public sealed class AuditAttribute : ServiceFilterAttribute
//{
//    public AuditAttribute(string actionDescription) : base(typeof(AuditActionFilter))
//    {
//        Arguments = [actionDescription];
//    }
//}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class AuditAttribute : ActionFilterAttribute
{
    private readonly string _actionDescription;

    public AuditAttribute(string actionDescription)
    {
        _actionDescription = actionDescription;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        IAuditLogService auditLogService = context.HttpContext.RequestServices.GetRequiredService<IAuditLogService>();
        ILogger<AuditAttribute> logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AuditAttribute>>();

        ActionExecutedContext executedContext = null;

        try
        {
            executedContext = await next();

            if (executedContext.Exception == null)
            {
                await auditLogService.LogAsync(_actionDescription);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in audit attribute for action: {Action}", _actionDescription);
            Console.WriteLine(ex);
        }
    }
}
