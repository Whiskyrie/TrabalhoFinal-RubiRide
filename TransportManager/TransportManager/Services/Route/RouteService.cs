// RouteService.cs
namespace TransportManager.Services;

public class RouteService : IRouteService
{
    private readonly IRouteRepository _routeRepository;
    private readonly ILogger<RouteService> _logger;
    private readonly CityDistances _cityDistances;

    public RouteService(IRouteRepository routeRepository, ILogger<RouteService> logger)
    {
        _routeRepository = routeRepository ?? throw new ArgumentNullException(nameof(routeRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cityDistances = new CityDistances();
    }

    public async Task<List<Route>> GetAllRoutesAsync()
    {
        try
        {
            return await _routeRepository.GetAllRoutesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todas as rotas");
            return new List<Route>();
        }
    }

    public async Task<Route?> GetRouteByIdAsync(string id)
    {
        try
        {
            return await _routeRepository.GetRouteByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter rota com ID {Id}", id);
            return null;
        }
    }

    public async Task<bool> AddRouteAsync(Route route)
    {
        if (route == null)
            throw new ArgumentNullException(nameof(route));

        var validationResults = route.Validate();
        if (validationResults.Count > 0)
        {
            var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
            _logger.LogWarning("Tentativa de adicionar rota inválida: {Errors}", errors);
            return false;
        }

        try
        {
            // Atualizar a distância com base nas cidades
            UpdateRouteDistance(route);
            
            // Calcular a duração estimada se não estiver definida
            if (route.EstimatedDuration <= TimeSpan.Zero)
            {
                route.CalculateEstimatedDuration();
            }

            await _routeRepository.AddRouteAsync(route);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar rota de {Start} para {End}", 
                route.StartLocation, route.EndLocation);
            return false;
        }
    }

    public async Task<bool> UpdateRouteAsync(Route route)
    {
        if (route == null)
            throw new ArgumentNullException(nameof(route));

        var validationResults = route.Validate();
        if (validationResults.Count > 0)
        {
            var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
            _logger.LogWarning("Tentativa de atualizar rota inválida: {Errors}", errors);
            return false;
        }

        try
        {
            // Atualizar a distância com base nas cidades
            UpdateRouteDistance(route);
            
            // Recalcular a duração estimada
            route.CalculateEstimatedDuration();

            await _routeRepository.UpdateRouteAsync(route);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar rota {Id}", route.Id);
            return false;
        }
    }

    public async Task<bool> RemoveRouteAsync(string id)
    {
        try
        {
            var route = await _routeRepository.GetRouteByIdAsync(id);
            if (route == null)
            {
                _logger.LogWarning("Tentativa de remover rota inexistente com ID {Id}", id);
                return false;
            }

            await _routeRepository.RemoveRouteAsync(route);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover rota com ID {Id}", id);
            return false;
        }
    }

    public async Task<List<Route>> GetRoutesByDriverAsync(string driverId)
    {
        try
        {
            var allRoutes = await _routeRepository.GetAllRoutesAsync();
            return allRoutes.Where(r => r.Driver?.Id == driverId).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter rotas para o motorista {Id}", driverId);
            return new List<Route>();
        }
    }

    public async Task<List<Route>> GetRoutesByVehicleAsync(string vehicleId)
    {
        try
        {
            var allRoutes = await _routeRepository.GetAllRoutesAsync();
            return allRoutes.Where(r => r.Vehicle?.Id == vehicleId).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter rotas para o veículo {Id}", vehicleId);
            return new List<Route>();
        }
    }

    public Task<double> CalculateDistanceAsync(string startLocation, string endLocation)
    {
        double distance = _cityDistances.GetDistance(startLocation, endLocation);
        return Task.FromResult(distance >= 0 ? distance : 0);
    }

    public Task<TimeSpan> CalculateEstimatedDurationAsync(double distance)
    {
        if (distance <= 0)
            return Task.FromResult(TimeSpan.Zero);

        // Assumindo velocidade média de 80 km/h
        double averageSpeed = 80;
        double hours = distance / averageSpeed;
        return Task.FromResult(TimeSpan.FromHours(hours));
    }

    // Método auxiliar para atualizar a distância de uma rota
    private void UpdateRouteDistance(Route route)
    {
        if (string.IsNullOrEmpty(route.StartLocation) || string.IsNullOrEmpty(route.EndLocation))
            return;
            
        double distance = _cityDistances.GetDistance(route.StartLocation, route.EndLocation);
        if (distance >= 0)
        {
            route.Distance = distance;
        }
    }
}