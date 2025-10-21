using ECommerce.BuildingBlocks.EventBus.Base;
using ECommerce.BuildingBlocks.EventBus.RabbitMQ;
using ECommerce.BuildingBlocks.Shared.Kernel.Auth.Options;
using ECommerce.BuildingBlocks.Shared.Kernel.Middlewares;
using ECommerce.OrderService.Application.Common.Interfaces;
using ECommerce.OrderService.Application.Context.PostgreSQL;
using ECommerce.OrderService.Domain.Repositories;
using ECommerce.OrderService.Infrastructure.Context.PostgreSQL;
using ECommerce.OrderService.Infrastructure.Repositories;
using ECommerce.OrderService.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using Space.DependencyInjection;

namespace ECommerce.OrderService.API;

public static class DependecyInjection
{
    private const string SectionName = "PostgreSQL";

    public static IServiceCollection InstallServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        #region HealthChecks
        services
            .AddSingleton<IConnection>(sp =>
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(configuration.GetSection("EventBusConfig")["ConnectionString"]),
                };
                return factory.CreateConnectionAsync().GetAwaiter().GetResult();
            })
            .AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("PostgreSQL"))
            .AddRabbitMQ();
        #endregion

        #region OpenApi
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
                {
                    ["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "JWT Authorization header using the Bearer scheme."
                    }
                };

                document.SecurityRequirements = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                        }
                    }
                };

                return Task.CompletedTask;
            });
        });
        #endregion

        #region DB
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString(SectionName));
        });
        #endregion

        #region RabbitMQ
        var eventBusConfig = configuration
            .GetSection("EventBusConfig")
            .Get<EventBusConfig>();

        services.AddRabbitMQEventBus(eventBusConfig!, new[] { typeof(Program).Assembly });
        #endregion

        #region Interfaces
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IAppDbContext, AppDbContext>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        #endregion

        #region Space
        services.AddSpace(configuration =>
        {
            configuration.ServiceLifetime = ServiceLifetime.Scoped;
        });
        #endregion

        #region Exceptions
        services.AddScoped<ExceptionMiddleware>();
        #endregion

        #region Auth
        services.AddHttpContextAccessor();

        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.Configure<JwtOptions>(configuration.GetSection("JwtSettings"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer();

        services.AddAuthorization();
        #endregion

        return services;
    }
}
