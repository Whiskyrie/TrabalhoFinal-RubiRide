// DriverService.cs
namespace TransportManager.Services;

public class DriverService : IDriverService
{
    private readonly IDriverRepository _driverRepository;
    private readonly ILogger<DriverService> _logger;

    public DriverService(IDriverRepository driverRepository, ILogger<DriverService> logger)
    {
        _driverRepository = driverRepository ?? throw new ArgumentNullException(nameof(driverRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<Driver>> GetAllDriversAsync()
    {
        try
        {
            return await _driverRepository.GetAllDriversAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todos os motoristas");
            return new List<Driver>();
        }
    }

    public async Task<Driver?> GetDriverByIdAsync(string id)
    {
        try
        {
            return await _driverRepository.GetDriverByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter motorista com ID {Id}", id);
            return null;
        }
    }

    public async Task<bool> AddDriverAsync(Driver driver)
    {
        if (driver == null)
            throw new ArgumentNullException(nameof(driver));

        if (!driver.IsValid(out var validationResults))
        {
            var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
            _logger.LogWarning("Tentativa de adicionar motorista inválido: {Errors}", errors);
            return false;
        }

        try
        {
            await _driverRepository.AddDriverAsync(driver);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar motorista {Name}", driver.Name);
            return false;
        }
    }

    public async Task<bool> UpdateDriverAsync(Driver driver)
    {
        if (driver == null)
            throw new ArgumentNullException(nameof(driver));

        if (!driver.IsValid(out var validationResults))
        {
            var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
            _logger.LogWarning("Tentativa de atualizar motorista inválido: {Errors}", errors);
            return false;
        }

        try
        {
            await _driverRepository.UpdateDriverAsync(driver);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar motorista {Id}", driver.Id);
            return false;
        }
    }

    public async Task<bool> RemoveDriverAsync(string id)
    {
        try
        {
            var driver = await _driverRepository.GetDriverByIdAsync(id);
            if (driver == null)
            {
                _logger.LogWarning("Tentativa de remover motorista inexistente com ID {Id}", id);
                return false;
            }

            await _driverRepository.RemoveDriverAsync(driver);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover motorista com ID {Id}", id);
            return false;
        }
    }

    public async Task<bool> IsDriverAvailableForRouteAsync(string driverId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var driver = await _driverRepository.GetDriverByIdAsync(driverId);
            if (driver == null)
            {
                return false;
            }

            // Verificar se o motorista está disponível (não está em outra rota no mesmo período)
            // Esta lógica deve ser implementada considerando as rotas existentes
            // Por enquanto, apenas verificamos o status do motorista
            return driver.Status == DriverStatus.Available;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar disponibilidade do motorista {Id}", driverId);
            return false;
        }
    }

    public async Task<List<Driver>> GetAvailableDriversAsync()
    {
        try
        {
            var allDrivers = await _driverRepository.GetAllDriversAsync();
            return allDrivers.Where(d => d.Status == DriverStatus.Available).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter motoristas disponíveis");
            return new List<Driver>();
        }
    }
}