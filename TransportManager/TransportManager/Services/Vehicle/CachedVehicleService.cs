using TransportManager.Services;

public class CachedVehicleService(
    IVehicleService vehicleService,
    ICacheService cacheService,
    ILogger<CachedVehicleService> logger) : IVehicleService
{
    private readonly IVehicleService _vehicleService = vehicleService ?? throw new ArgumentNullException(nameof(vehicleService));
    private readonly ICacheService _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    private readonly ILogger<CachedVehicleService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    
    // Chaves padrão para o cache
    private const string AllVehiclesCacheKey = "AllVehicles";
    private const string VehicleByIdCacheKeyPrefix = "Vehicle_";

    public async Task<List<Vehicle>> GetAllVehiclesAsync()
    {
        // Tentar obter do cache primeiro
        var cachedVehicles = _cacheService.Get<List<Vehicle>>(AllVehiclesCacheKey);
        if (cachedVehicles != null)
        {
            _logger.LogInformation("Retornando lista de veículos do cache");
            return cachedVehicles;
        }

        // Se não estiver no cache, buscar do serviço real
        var vehicles = await _vehicleService.GetAllVehiclesAsync();
        
        // Armazenar no cache por 5 minutos
        _cacheService.Set(AllVehiclesCacheKey, vehicles, 5);
        
        return vehicles;
    }

    public async Task<Vehicle?> GetVehicleByIdAsync(string id)
    {
        var cacheKey = $"{VehicleByIdCacheKeyPrefix}{id}";
        
        // Tentar obter do cache primeiro
        var cachedVehicle = _cacheService.Get<Vehicle>(cacheKey);
        if (cachedVehicle != null)
        {
            _logger.LogInformation("Retornando veículo {Id} do cache", id);
            return cachedVehicle;
        }

        // Se não estiver no cache, buscar do serviço real
        var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
        
        // Se encontrado, armazenar no cache
        if (vehicle != null)
        {
            _cacheService.Set(cacheKey, vehicle, 5);
        }
        
        return vehicle;
    }

    public async Task<bool> AddVehicleAsync(Vehicle vehicle)
    {
        var result = await _vehicleService.AddVehicleAsync(vehicle);
        
        if (result)
        {
            // Invalidar cache relevante
            _cacheService.Remove(AllVehiclesCacheKey);
        }
        
        return result;
    }

    public async Task<bool> UpdateVehicleAsync(Vehicle vehicle)
    {
        var result = await _vehicleService.UpdateVehicleAsync(vehicle);
        
        if (result)
        {
            // Invalidar cache relevante
            _cacheService.Remove(AllVehiclesCacheKey);
            _cacheService.Remove($"{VehicleByIdCacheKeyPrefix}{vehicle.Id}");
        }
        
        return result;
    }

    public async Task<bool> RemoveVehicleAsync(string id)
    {
        var result = await _vehicleService.RemoveVehicleAsync(id);
        
        if (result)
        {
            // Invalidar cache relevante
            _cacheService.Remove(AllVehiclesCacheKey);
            _cacheService.Remove($"{VehicleByIdCacheKeyPrefix}{id}");
        }
        
        return result;
    }

    public Task<bool> IsVehicleAvailableForRouteAsync(string vehicleId, DateTime startDate, DateTime endDate)
    {
        // Não faz sentido cachear este método, então passar diretamente para o serviço real
        return _vehicleService.IsVehicleAvailableForRouteAsync(vehicleId, startDate, endDate);
    }
}