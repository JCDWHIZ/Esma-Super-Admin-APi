using System.Reflection;
using Application;
using Hangfire;
using HealthChecks.UI.Client;
using Infrastructure;
using Infrastructure.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Web.Api;
using Web.Api.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));


string[]? allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
if (allowedOrigins is null || allowedOrigins.Length == 0)
{
    throw new InvalidOperationException("Allowed origins for CORS must be configured in appsettings.json.");
}
builder.Services.AddCors(options => options.AddPolicy("AppCorsPolicy", policy => policy.WithOrigins(allowedOrigins!)
              .AllowAnyHeader()
              .AllowAnyMethod()));

builder.Services.AddSwaggerGenWithAuth();

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

WebApplication app = builder.Build();

app.UseSwaggerWithUi();
app.MapEndpoints();

//if (app.Environment.IsDevelopment())
//{
//}
app.ApplyMigrations();
app.UseHttpsRedirection();
app.UseHsts();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseHttpsRedirection();

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseCors("AppCorsPolicy");

app.UseMiddleware<ApiResponseMiddleware>();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard("/hangfire");
RecurringJob.AddOrUpdate<SubscriptionCheckJob>(
              "check-subscriptions",
              job => job.CheckSubscriptionsAsync(),
              "0 */5 * * *");
//"* * * * *");

await app.RunAsync();

// REMARK: Required for functional and integration tests to work.
namespace Web.Api
{
    public class Program;
}
