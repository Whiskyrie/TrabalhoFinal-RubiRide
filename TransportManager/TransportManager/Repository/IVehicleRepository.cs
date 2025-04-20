// IVehicleRepository.cs
namespace TransportManager.Repository;

public interface IVehicleRepository
{
    Task<List<Vehicle>> GetAllVehiclesAsync();
    Task AddVehicleAsync(Vehicle vehicle);
    Task UpdateVehicleAsync(Vehicle vehicle);
    Task RemoveVehicleAsync(Vehicle vehicle);
    Task<Vehicle?> GetVehicleByIdAsync(string id);
}