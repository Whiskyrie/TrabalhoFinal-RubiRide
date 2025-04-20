using TransportManager.Services;

public class MemoryCacheService : ICacheService, IDisposable
{
    private class CacheItem
    {
        public required object Value { get; set; }
        public DateTime ExpirationTime { get; set; }
    }

    private readonly Dictionary<string, CacheItem> _cache = new();
    private readonly object _lock = new();
    private readonly Timer _cleanupTimer;

    public MemoryCacheService()
    {
        // Configurar timer para limpar itens expirados a cada minuto
        _cleanupTimer = new Timer(CleanupCache, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    public T? Get<T>(string key)
    {
        lock (_lock)
        {
            if (_cache.TryGetValue(key, out var item) && item.ExpirationTime > DateTime.UtcNow)
            {
                return (T)item.Value;
            }

            return default;
        }
    }

    public void Set<T>(string key, T value, int expirationInMinutes = 10)
    {
        lock (_lock)
        {
            var expirationTime = DateTime.UtcNow.AddMinutes(expirationInMinutes);
            
            _cache[key] = new CacheItem
            {
                Value = value!,
                ExpirationTime = expirationTime
            };
        }
    }

    public void Remove(string key)
    {
        lock (_lock)
        {
            _cache.Remove(key);
        }
    }

    public bool Contains(string key)
    {
        lock (_lock)
        {
            return _cache.TryGetValue(key, out var item) && item.ExpirationTime > DateTime.UtcNow;
        }
    }

    private void CleanupCache(object? state)
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            var expiredKeys = _cache.Where(kvp => kvp.Value.ExpirationTime < now)
                                    .Select(kvp => kvp.Key)
                                    .ToList();

            foreach (var key in expiredKeys)
            {
                _cache.Remove(key);
            }
        }
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}