namespace TransportManager.Services;

public interface ICacheService
{
    T? Get<T>(string key);

    void Set<T>(string key, T value, int expirationInMinutes = 10);
    
    void Remove(string key);
    
    bool Contains(string key);
}
