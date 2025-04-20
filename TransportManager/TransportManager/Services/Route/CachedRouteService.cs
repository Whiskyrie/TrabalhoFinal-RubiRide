// CachedRouteService.cs
namespace TransportManager.Services;

public class CachedRouteService : IRouteService
{
    private readonly IRouteService _routeService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedRouteService> _logger;
    
    // Chaves padrão para o cache
    private const string AllRoutesCacheKey = "AllRoutes";
    private const string RouteByIdCacheKeyPrefix = "Route_";
    private const string RoutesByDriverPrefix = "DriverRoutes_";
    private const string RoutesByVehiclePrefix = "VehicleRoutes_";
    private const string DistanceCacheKeyPrefix = "Distance_";
    
    public CachedRouteService(
        IRouteService routeService, 
        ICacheService cacheService,
        ILogger<CachedRouteService> logger)
    {
        _routeService = routeService ?? throw new ArgumentNullException(nameof(routeService));
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<Route>> GetAllRoutesAsync()
    {
        // Tentar obter do cache primeiro
        var cachedRoutes = _cacheService.Get<List<Route>>(AllRoutesCacheKey);
        if (cachedRoutes != null)
        {
            _logger.LogInformation("Retornando lista de rotas do cache");
            return cachedRoutes;
        }

        // Se não estiver no cache, buscar do serviço real
        var routes = await _routeService.GetAllRoutesAsync();
        
        // Armazenar no cache por 5 minutos
        _cacheService.Set(AllRoutesCacheKey, routes, 5);
        
        return routes;
    }

    public async Task<Route?> GetRouteByIdAsync(string id)
    {
        var cacheKey = $"{RouteByIdCacheKeyPrefix}{id}";
        
        // Tentar obter do cache primeiro
        var cachedRoute = _cacheService.Get<Route>(cacheKey);
        if (cachedRoute != null)
        {
            _logger.LogInformation("Retornando rota {Id} do cache", id);
            return cachedRoute;
        }

        // Se não estiver no cache, buscar do serviço real
        var route = await _routeService.GetRouteByIdAsync(id);
        
        // Se encontrado, armazenar no cache
        if (route != null)
        {
            _cacheService.Set(cacheKey, route, 5);
        }
        
        return route;
    }

    public async Task<bool> AddRouteAsync(Route route)
    {
        var result = await _routeService.AddRouteAsync(route);
        
        if (result)
        {
            // Invalidar cache relevante
            InvalidateRouteCaches(route);
        }
        
        return result;
    }

    public async Task<bool> UpdateRouteAsync(Route route)
    {
        var result = await _routeService.UpdateRouteAsync(route);
        
        if (result)
        {
            // Invalidar cache relevante
            InvalidateRouteCaches(route);
        }
        
        return result;
    }

    public async Task<bool> RemoveRouteAsync(string id)
    {
        // Obter a rota antes de remover para saber quais caches invalidar
        var route = await _routeService.GetRouteByIdAsync(id);
        var result = await _routeService.RemoveRouteAsync(id);
        
        if (result && route != null)
        {
            // Invalidar cache relevante
            InvalidateRouteCaches(route);
        }
        
        return result;
    }

    public async Task<List<Route>> GetRoutesByDriverAsync(string driverId)
    {
        var cacheKey = $"{RoutesByDriverPrefix}{driverId}";
        
        // Tentar obter do cache primeiro
        var cachedRoutes = _cacheService.Get<List<Route>>(cacheKey);
        if (cachedRoutes != null)
        {
            _logger.LogInformation("Retornando rotas do motorista {Id} do cache", driverId);
            return cachedRoutes;
        }

        // Se não estiver no cache, buscar do serviço real
        var routes = await _routeService.GetRoutesByDriverAsync(driverId);
        
        // Armazenar no cache
        _cacheService.Set(cacheKey, routes, 5);
        
        return routes;
    }

    public async Task<List<Route>> GetRoutesByVehicleAsync(string vehicleId)
    {
        var cacheKey = $"{RoutesByVehiclePrefix}{vehicleId}";
        
        // Tentar obter do cache primeiro
        var cachedRoutes = _cacheService.Get<List<Route>>(cacheKey);
        if (cachedRoutes != null)
        {
            _logger.LogInformation("Retornando rotas do veículo {Id} do cache", vehicleId);
            return cachedRoutes;
        }

        // Se não estiver no cache, buscar do serviço real
        var routes = await _routeService.GetRoutesByVehicleAsync(vehicleId);
        
        // Armazenar no cache
        _cacheService.Set(cacheKey, routes, 5);
        
        return routes;
    }

    public async Task<double> CalculateDistanceAsync(string startLocation, string endLocation)
    {
        if (string.IsNullOrEmpty(startLocation) || string.IsNullOrEmpty(endLocation))
            return 0;
            
        var cacheKey = $"{DistanceCacheKeyPrefix}{startLocation}_{endLocation}";
        
        // Tentar obter do cache primeiro
        var cachedDistance = _cacheService.Get<double?>(cacheKey);
        if (cachedDistance.HasValue)
        {
            return cachedDistance.Value;
        }

        // Se não estiver no cache, calcular
        var distance = await _routeService.CalculateDistanceAsync(startLocation, endLocation);
        
        // Armazenar no cache (distâncias são estáticas, então podemos cache por mais tempo)
        _cacheService.Set(cacheKey, distance, 60); // 1 hora
        
        return distance;
    }

    public Task<TimeSpan> CalculateEstimatedDurationAsync(double distance)
    {
        // Cálculo simples que não precisa ser cacheado
        return _routeService.CalculateEstimatedDurationAsync(distance);
    }

    // Método auxiliar para invalidar todos os caches relevantes para uma rota
    private void InvalidateRouteCaches(Route route)
    {
        _cacheService.Remove(AllRoutesCacheKey);
        _cacheService.Remove($"{RouteByIdCacheKeyPrefix}{route.Id}");
        
        if (route.Driver != null)
        {
            _cacheService.Remove($"{RoutesByDriverPrefix}{route.Driver.Id}");
        }
        
        if (route.Vehicle != null)
        {
            _cacheService.Remove($"{RoutesByVehiclePrefix}{route.Vehicle.Id}");
        }
    }
}