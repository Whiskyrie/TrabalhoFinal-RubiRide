// VehicleService.cs
namespace TransportManager.Services;

public class VehicleService(IVehicleRepository vehicleRepository, ILogger<VehicleService> logger) : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository));
    private readonly ILogger<VehicleService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<List<Vehicle>> GetAllVehiclesAsync()
    {
        try
        {
            return await _vehicleRepository.GetAllVehiclesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todos os veículos");
            return new List<Vehicle>();
        }
    }

    public async Task<Vehicle?> GetVehicleByIdAsync(string id)
    {
        try
        {
            return await _vehicleRepository.GetVehicleByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter veículo com ID {Id}", id);
            return null;
        }
    }

    public async Task<bool> AddVehicleAsync(Vehicle vehicle)
    {
        if (vehicle == null)
            throw new ArgumentNullException(nameof(vehicle));

        if (!vehicle.IsValid(out var validationResults))
        {
            var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
            _logger.LogWarning("Tentativa de adicionar veículo inválido: {Errors}", errors);
            return false;
        }

        try
        {
            await _vehicleRepository.AddVehicleAsync(vehicle);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar veículo {Model}", vehicle.Model);
            return false;
        }
    }

    public async Task<bool> UpdateVehicleAsync(Vehicle vehicle)
    {
        if (vehicle == null)
            throw new ArgumentNullException(nameof(vehicle));

        if (!vehicle.IsValid(out var validationResults))
        {
            var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
            _logger.LogWarning("Tentativa de atualizar veículo inválido: {Errors}", errors);
            return false;
        }

        try
        {
            await _vehicleRepository.UpdateVehicleAsync(vehicle);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar veículo {Id}", vehicle.Id);
            return false;
        }
    }

    public async Task<bool> RemoveVehicleAsync(string id)
    {
        try
        {
            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(id);
            if (vehicle == null)
            {
                _logger.LogWarning("Tentativa de remover veículo inexistente com ID {Id}", id);
                return false;
            }

            await _vehicleRepository.RemoveVehicleAsync(vehicle);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover veículo com ID {Id}", id);
            return false;
        }
    }

    public Task<bool> IsVehicleAvailableForRouteAsync(string vehicleId, DateTime startDate, DateTime endDate)
    {
        // Implementar lógica para verificar se o veículo está disponível para uma rota no período especificado
        // Esta é uma funcionalidade que seria útil adicionar ao sistema
        return Task.FromResult(true); // Implementação simplificada
    }
}