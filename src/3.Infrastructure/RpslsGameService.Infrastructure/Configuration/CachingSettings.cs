using RpslsGameService.Domain;

namespace RpslsGameService.Infrastructure.Configuration;

public class CachingSettings
{
    public const string SectionName = Constants.ConfigurationSections.Caching;
    
    public TimeSpan DefaultExpiration { get; set; }
    public bool EnableDistributedCache { get; set; }
}