using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RpslsGameService.Application.Interfaces;
using RpslsGameService.Infrastructure.Caching;
using RpslsGameService.Infrastructure.Configuration;
using RpslsGameService.Infrastructure.ExternalServices;
using RpslsGameService.Infrastructure.Persistence;
using Serilog;
using System.Net;

namespace RpslsGameService.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConfiguration(configuration);
        services.AddExternalServices(configuration);
        services.AddPersistence();
        services.AddCaching();
        services.AddLogging();

        return services;
    }

    private static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ExternalApiSettings>(configuration.GetSection(ExternalApiSettings.SectionName));
        services.Configure<CachingSettings>(configuration.GetSection(CachingSettings.SectionName));
        services.Configure<HttpClientSettings>(configuration.GetSection(HttpClientSettings.SectionName));

        return services;
    }

    private static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        var externalApiSettings = configuration.GetSection(ExternalApiSettings.SectionName).Get<ExternalApiSettings>();
        var randomNumberSettings = externalApiSettings?.RandomNumberService ?? new RandomNumberServiceSettings();
        var httpClientSettings = configuration.GetSection(HttpClientSettings.SectionName).Get<HttpClientSettings>() ?? new HttpClientSettings();

        services.AddHttpClient<IRandomNumberService, HttpRandomNumberService>(client =>
        {
            client.BaseAddress = new Uri(randomNumberSettings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(randomNumberSettings.TimeoutSeconds);
            
            // Optimize HTTP client settings for performance
            if (httpClientSettings.EnableCompression)
            {
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            }
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
        })
        .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            // Enable response compression
            AutomaticDecompression = httpClientSettings.EnableCompression
                ? DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli
                : DecompressionMethods.None,
            
            // Optimize connection pooling
            MaxConnectionsPerServer = httpClientSettings.MaxConnectionsPerServer,
            
            // Configure connection lifetime
            PooledConnectionLifetime = TimeSpan.FromMinutes(httpClientSettings.ConnectionLifetimeMinutes),
            
            // Enable connection keep-alive
            UseCookies = httpClientSettings.UseCookies
        });
        // Note: Resilience patterns (retry, circuit breaker, timeout) are handled
        // by the comprehensive Polly pipeline within HttpRandomNumberService itself

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddSingleton<IGameSessionRepository, InMemoryGameSessionRepository>();

        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, InMemoryCacheService>();

        return services;
    }

    private static IServiceCollection AddLogging(this IServiceCollection services)
    {
        var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/rpsls-game-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        services.AddSingleton<Serilog.ILogger>(logger);

        return services;
    }

}