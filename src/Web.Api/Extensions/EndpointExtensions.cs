using System.Reflection;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Web.Api.Endpoints;

namespace Web.Api.Extensions;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        ServiceDescriptor[] serviceDescriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static IApplicationBuilder MapEndpoints(
        this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder = null)
    {
        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }

    public static RouteHandlerBuilder HasPermission(this RouteHandlerBuilder app, string permission)
    {
        return app.RequireAuthorization(permission);
    }

    public static RouteHandlerBuilder WithAudit(this RouteHandlerBuilder builder, string actionDescription)
    {
        return builder.AddEndpointFilter(async (context, next) =>
        {
            object? result = await next(context);

            if (context.HttpContext.Response.StatusCode >= 200 && context.HttpContext.Response.StatusCode < 300)
            {
                IAuditLogService auditService = context.HttpContext.RequestServices.GetRequiredService<IAuditLogService>();
                await auditService.LogAsync(actionDescription);
            }

            return result;
        });
    }
}
