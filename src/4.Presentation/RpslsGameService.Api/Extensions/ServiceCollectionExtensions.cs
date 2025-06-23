using FluentValidation;
using MediatR;
using Microsoft.OpenApi.Models;
using RpslsGameService.Api.Configuration;
using RpslsGameService.Application.Behaviors;
using RpslsGameService.Application.CQRS.Commands;
using RpslsGameService.Application.Mappings;
using RpslsGameService.Application.Validators;
using RpslsGameService.Domain.Interfaces;
using RpslsGameService.Domain.Services;
using System.Reflection;

namespace RpslsGameService.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddAuthorization();

        // Configure Swagger with authentication support
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { 
                Title = "RPSLS Game API", 
                Version = "v1",
                Description = "Rock, Paper, Scissors, Lizard, Spock Game API with JWT and API Key authentication"
            });
            
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            // Add API Key authentication to Swagger
            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "API Key needed to access the endpoints. Example: \"X-API-Key: {key}\"",
                In = ParameterLocation.Header,
                Name = "X-API-Key",
                Type = SecuritySchemeType.ApiKey
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "ApiKey"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        // Enhanced CORS configuration for security
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins("http://localhost:3000", "https://localhost:3001")
                    .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                    .WithHeaders("Content-Type", "Authorization", "X-API-Key")
                    .AllowCredentials();
            });

            // Add a more permissive policy for development
            options.AddPolicy("Development", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        // Configure settings
        services.Configure<ApiKeySettings>(configuration.GetSection(ApiKeySettings.SectionName));

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(PlayGameCommandHandler).Assembly));
        
        // AutoMapper configuration - scans assembly for mapping profiles
        services.AddAutoMapper(typeof(GameMappingProfile).Assembly);
        
        services.AddValidatorsFromAssemblyContaining<PlayGameRequestValidator>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        // Domain services are stateless and can be singletons for better performance
        services.AddSingleton<IGameLogicService, GameLogicService>();
        services.AddSingleton<IChoiceGenerationService, ChoiceGenerationService>();

        return services;
    }
}