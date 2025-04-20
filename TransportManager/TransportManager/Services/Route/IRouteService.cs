// IRouteService.cs
namespace TransportManager.Services;

public interface IRouteService
{
    Task<List<Route>> GetAllRoutesAsync();
    Task<Route?> GetRouteByIdAsync(string id);
    Task<bool> AddRouteAsync(Route route);
    Task<bool> UpdateRouteAsync(Route route);
    Task<bool> RemoveRouteAsync(string id);
    Task<List<Route>> GetRoutesByDriverAsync(string driverId);
    Task<List<Route>> GetRoutesByVehicleAsync(string vehicleId);
    Task<double> CalculateDistanceAsync(string startLocation, string endLocation);
    Task<TimeSpan> CalculateEstimatedDurationAsync(double distance);
}