using System.Text;
using System.Text.Json.Serialization;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.BackgroundJobs;
using Application.Interfaces;
using Application.Interfaces.Services;
using Domain.Schools;
using Domain.Users;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.Authentication;
using Infrastructure.Authorization;
using Infrastructure.Database;
using Infrastructure.DomainEvents;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Time;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Refit;
using SharedKernel;
using SharedKernel.Enums;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddServices()
            .AddDatabase(configuration)
            .AddHealthChecks(configuration)
            .AddAuthenticationInternal(configuration)
            .AddConfiguration(configuration)
            .AddKafkaServices(configuration)
            .AddAuthorizationInternal();

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

        services.AddSingleton<ITokenService, TokenService>();

        services.AddTransient<IClaimsTransformation, KeycloakRoleClaimsTransformer>();

        // Register application services
        services.AddScoped<IEmailService, EmailService>();

        // services.AddIdentityCore<ApplicationUser>()
        //     .AddRoles<IdentityRole>()
        //     .AddEntityFrameworkStores<ApplicationDbContext>()
        //     .AddApiEndpoints();


        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IExportService, ExportService>();
        services.AddScoped<IExportStrategy>(sp =>
            new DynamicExportStrategy<User>(
            sp.GetRequiredService<IGenericRepository<User>>(),
            AdminModule.USER
            )
);

        services.AddScoped<IExportStrategy>(sp =>
            new DynamicExportStrategy<Schools>(
                sp.GetRequiredService<IGenericRepository<Schools>>(),
                AdminModule.SCHOOL
            )
        );

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddSingleton(TimeProvider.System);
        // services.AddTransient<IIdentityService, IdentityService>();

        return services;
    }

    private static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));


        // builder.Services.AddHttpClient<IEmailService, EmailServices>();
        services.AddRefitClient<IKeycloakApi>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(configuration["Keycloak:BaseUrl"]!));
        services.AddScoped<IKeycloakOrganizationService, KeycloakOrganizationService>();
        services.AddScoped<KeycloakService>();

        services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(configuration.GetConnectionString("Database"))));
        services.AddHangfireServer();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseNpgsql(connectionString, npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("Database")!);

        services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        return services;
    }

    public static IServiceCollection AddKafkaServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        services.AddScoped<IMessageProducer, KafkaProducer>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddHostedService<TenantResponseHandlerService>();

        return services;
    }

    private static IServiceCollection AddAuthenticationInternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //     .AddJwtBearer(o =>
        //     {
        //         o.RequireHttpsMetadata = false;
        //         o.TokenValidationParameters = new TokenValidationParameters
        //         {
        //             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
        //             ValidIssuer = configuration["Jwt:Issuer"],
        //             ValidAudience = configuration["Jwt:Audience"],
        //             ClockSkew = TimeSpan.Zero
        //         };
        //     });

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["Keycloak:Authority"];
                options.Audience = configuration.GetSection("Keycloak:Audiences").GetChildren().FirstOrDefault()?.Value
                      ?? configuration["Keycloak:ClientId"];

                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Keycloak:Authority"],
                    // ValidateAudience = false,
                    ValidAudiences = [configuration["Keycloak:ClientId"], "account"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };

                options.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = OnTokenValidatedHandler
                };

            });

        services.AddAuthorization(options =>
        {
            // options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrator"));
            // options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"));
            // options.AddPolicy("CanCreateBlog", policy => policy.RequireRole("CreateBlog"));
            // options.AddPolicy("CanDelteBlog", policy => policy.RequireRole("DeleteBlog"));
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrator"));
            options.AddPolicy("ManagerOnly", policy => policy.RequireRole("MANAGER"));
            options.AddPolicy("CanCreateBlog", policy => policy.RequireRole("CREATE_BLOG"));
            options.AddPolicy("CanDeleteBlog", policy => policy.RequireRole("DELETE_BLOG"));
        });

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenProvider, TokenProvider>();

        return services;
    }

    private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
    {
        services.AddAuthorization();

        services.AddScoped<PermissionProvider>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }

    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Web.Api");
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

            // Get connection string
            string? connectionString = configuration.GetConnectionString("Database");

            // Configure DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder
                .UseNpgsql(connectionString, npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default))
                .UseSnakeCaseNamingConvention();

            // Create a mock/null domain events dispatcher for design time
            var mockDomainEventsDispatcher = new MockDomainEventsDispatcher();

            return new ApplicationDbContext(optionsBuilder.Options, mockDomainEventsDispatcher);
        }
    }

    // Mock implementation for design-time use only
    public class MockDomainEventsDispatcher : IDomainEventsDispatcher
    {
        public Task DispatchAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    private static Task OnTokenValidatedHandler(TokenValidatedContext context)
    {

        if (context.Principal == null || !context.Principal.Identities.Any(identity => identity.IsAuthenticated))
        {
            return Task.CompletedTask;
        }
        else
        {
            var claims = context.Principal.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            Console.WriteLine("Claims: " + string.Join(", ", claims));
        }

        return Task.CompletedTask;
    }
}
