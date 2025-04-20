// IVehicleService.cs
namespace TransportManager.Services;

public interface IVehicleService
{
    Task<List<Vehicle>> GetAllVehiclesAsync();
    Task<Vehicle?> GetVehicleByIdAsync(string id);
    Task<bool> AddVehicleAsync(Vehicle vehicle);
    Task<bool> UpdateVehicleAsync(Vehicle vehicle);
    Task<bool> RemoveVehicleAsync(string id);
    Task<bool> IsVehicleAvailableForRouteAsync(string vehicleId, DateTime startDate, DateTime endDate);
}