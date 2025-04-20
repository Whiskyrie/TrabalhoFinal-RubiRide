// IDriverService.cs
namespace TransportManager.Services;

public interface IDriverService
{
    Task<List<Driver>> GetAllDriversAsync();
    Task<Driver?> GetDriverByIdAsync(string id);
    Task<bool> AddDriverAsync(Driver driver);
    Task<bool> UpdateDriverAsync(Driver driver);
    Task<bool> RemoveDriverAsync(string id);
    Task<bool> IsDriverAvailableForRouteAsync(string driverId, DateTime startDate, DateTime endDate);
    Task<List<Driver>> GetAvailableDriversAsync();
}