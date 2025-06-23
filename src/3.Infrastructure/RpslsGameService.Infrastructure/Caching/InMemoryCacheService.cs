using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RpslsGameService.Infrastructure.Configuration;
using System.Text.Json;

namespace RpslsGameService.Infrastructure.Caching;

public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<InMemoryCacheService> _logger;
    private readonly CachingSettings _settings;

    public InMemoryCacheService(
        IMemoryCache memoryCache,
        IOptions<CachingSettings> cachingSettings,
        ILogger<InMemoryCacheService> logger)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = cachingSettings?.Value ?? throw new ArgumentNullException(nameof(cachingSettings));
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

        try
        {
            if (_memoryCache.TryGetValue(key, out var cachedValue) && cachedValue is T value)
            {
                _logger.LogDebug("Cache hit for key: {CacheKey}", key);
                return Task.FromResult<T?>(value);
            }

            _logger.LogDebug("Cache miss for key: {CacheKey}", key);
            return Task.FromResult<T?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving from cache for key: {CacheKey}", key);
            return Task.FromResult<T?>(null);
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        try
        {
            var cacheExpiration = expiration ?? _settings.DefaultExpiration;
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheExpiration,
                SlidingExpiration = TimeSpan.FromMinutes(2),
                Priority = CacheItemPriority.Normal
            };

            _memoryCache.Set(key, value, cacheEntryOptions);
            _logger.LogDebug("Cached value for key: {CacheKey} with expiration: {Expiration}", key, cacheExpiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache for key: {CacheKey}", key);
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

        try
        {
            _memoryCache.Remove(key);
            _logger.LogDebug("Removed cache entry for key: {CacheKey}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache entry for key: {CacheKey}", key);
        }

        return Task.CompletedTask;
    }

    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_memoryCache is MemoryCache memoryCache)
            {
                memoryCache.Compact(1.0);
                _logger.LogInformation("Cache cleared successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cache");
        }

        return Task.CompletedTask;
    }
}