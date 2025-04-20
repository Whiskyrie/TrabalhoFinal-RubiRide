// CachedDriverService.cs
namespace TransportManager.Services;

public class CachedDriverService : IDriverService
{
    private readonly IDriverService _driverService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedDriverService> _logger;
    
    // Chaves padrão para o cache
    private const string AllDriversCacheKey = "AllDrivers";
    private const string DriverByIdCacheKeyPrefix = "Driver_";
    private const string AvailableDriversCacheKey = "AvailableDrivers";
    
    public CachedDriverService(
        IDriverService driverService, 
        ICacheService cacheService,
        ILogger<CachedDriverService> logger)
    {
        _driverService = driverService ?? throw new ArgumentNullException(nameof(driverService));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<Driver>> GetAllDriversAsync()
    {
        // Tentar obter do cache primeiro
        var cachedDrivers = _cacheService.Get<List<Driver>>(AllDriversCacheKey);
        if (cachedDrivers != null)
        {
            _logger.LogInformation("Retornando lista de motoristas do cache");
            return cachedDrivers;
        }

        // Se não estiver no cache, buscar do serviço real
        var drivers = await _driverService.GetAllDriversAsync();
        
        // Armazenar no cache por 5 minutos
        _cacheService.Set(AllDriversCacheKey, drivers, 5);
        
        return drivers;
    }

    public async Task<Driver?> GetDriverByIdAsync(string id)
    {
        var cacheKey = $"{DriverByIdCacheKeyPrefix}{id}";
        
        // Tentar obter do cache primeiro
        var cachedDriver = _cacheService.Get<Driver>(cacheKey);
        if (cachedDriver != null)
        {
            _logger.LogInformation("Retornando motorista {Id} do cache", id);
            return cachedDriver;
        }

        // Se não estiver no cache, buscar do serviço real
        var driver = await _driverService.GetDriverByIdAsync(id);
        
        // Se encontrado, armazenar no cache
        if (driver != null)
        {
            _cacheService.Set(cacheKey, driver, 5);
        }
        
        return driver;
    }

    public async Task<bool> AddDriverAsync(Driver driver)
    {
        var result = await _driverService.AddDriverAsync(driver);
        
        if (result)
        {
            // Invalidar cache relevante
            _cacheService.Remove(AllDriversCacheKey);
            _cacheService.Remove(AvailableDriversCacheKey);
        }
        
        return result;
    }

    public async Task<bool> UpdateDriverAsync(Driver driver)
    {
        var result = await _driverService.UpdateDriverAsync(driver);
        
        if (result)
        {
            // Invalidar cache relevante
            _cacheService.Remove(AllDriversCacheKey);
            _cacheService.Remove($"{DriverByIdCacheKeyPrefix}{driver.Id}");
            _cacheService.Remove(AvailableDriversCacheKey);
        }
        
        return result;
    }

    public async Task<bool> RemoveDriverAsync(string id)
    {
        var result = await _driverService.RemoveDriverAsync(id);
        
        if (result)
        {
            // Invalidar cache relevante
            _cacheService.Remove(AllDriversCacheKey);
            _cacheService.Remove($"{DriverByIdCacheKeyPrefix}{id}");
            _cacheService.Remove(AvailableDriversCacheKey);
        }
        
        return result;
    }

    public Task<bool> IsDriverAvailableForRouteAsync(string driverId, DateTime startDate, DateTime endDate)
    {
        // Não faz sentido cachear este método, então passar diretamente para o serviço real
        return _driverService.IsDriverAvailableForRouteAsync(driverId, startDate, endDate);
    }

    public async Task<List<Driver>> GetAvailableDriversAsync()
    {
        // Tentar obter do cache primeiro
        var cachedDrivers = _cacheService.Get<List<Driver>>(AvailableDriversCacheKey);
        if (cachedDrivers != null)
        {
            _logger.LogInformation("Retornando lista de motoristas disponíveis do cache");
            return cachedDrivers;
        }

        // Se não estiver no cache, buscar do serviço real
        var drivers = await _driverService.GetAvailableDriversAsync();
        
        // Armazenar no cache por curto período (3 minutos) pois disponibilidade pode mudar rapidamente
        _cacheService.Set(AvailableDriversCacheKey, drivers, 3);
        
        return drivers;
    }
}