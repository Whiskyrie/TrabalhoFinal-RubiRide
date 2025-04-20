// IRouteRepository.cs
namespace TransportManager.Repository;

public interface IRouteRepository
{
    Task<List<Route>> GetAllRoutesAsync();
    Task AddRouteAsync(Route route);
    Task UpdateRouteAsync(Route route);
    Task RemoveRouteAsync(Route route);
    Task<Route?> GetRouteByIdAsync(string id);
}