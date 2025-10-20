using Application.Interfaces;
using Infrastructure.Services;
using Microsoft.OpenApi.Models;

namespace Web.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddScoped<IAuditLogService, AuditLogService>();
        //services.AddScoped<AuditActionFilter>();
        //services.AddTransient<AuditActionFilter>(provider =>
        //    // This won't work directly because we need the actionDescription parameter
        //    // This is why Option 1 is recommended
        //    throw new InvalidOperationException("Use TypeFilterAttribute instead of ServiceFilterAttribute"));
        services.AddSwaggerGen(options =>
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Esma Superadmin API",
            })
        );

        // REMARK: If you want to use Controllers, you'll need this.
        services.AddControllers();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
