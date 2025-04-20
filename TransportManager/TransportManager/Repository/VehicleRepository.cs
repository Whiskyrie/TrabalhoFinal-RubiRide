namespace TransportManager.Repository;

public class VehicleRepository : IVehicleRepository
{
  private readonly TransportDbContext _context;

  public VehicleRepository(TransportDbContext context)
  {
    _context = context ?? throw new ArgumentNullException(nameof(context));
  }

  public async Task<List<Vehicle>> GetAllVehiclesAsync()
  {
    return await _context.Vehicles.ToListAsync();
  }

  public async Task<Vehicle?> GetVehicleByIdAsync(string id)
  {
    return await _context.Vehicles.FindAsync(id);
  }

  public async Task AddVehicleAsync(Vehicle vehicle)
  {
    if (vehicle == null)
      throw new ArgumentNullException(nameof(vehicle));

    await _context.Vehicles.AddAsync(vehicle);
    await _context.SaveChangesAsync();
  }

  public async Task UpdateVehicleAsync(Vehicle vehicle)
  {
    if (vehicle == null)
      throw new ArgumentNullException(nameof(vehicle));

    try
    {
      var existingVehicle = await _context.Vehicles.FindAsync(vehicle.Id);
      if (existingVehicle == null)
        throw new InvalidOperationException($"Veículo com ID {vehicle.Id} não encontrado no banco de dados.");

      _context.Entry(existingVehicle).CurrentValues.SetValues(vehicle);
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException ex)
    {
      // Lidar com possíveis conflitos de concorrência
      var entry = ex.Entries.Single();
      var databaseValues = await entry.GetDatabaseValuesAsync();

      if (databaseValues == null)
        throw new InvalidOperationException("O veículo foi removido por outro usuário.", ex);

      // Log das diferenças para diagnóstico
      LogDatabaseConflict(entry, databaseValues);

      throw; // Relançar a exceção para que a camada superior possa lidar com ela
    }
  }

  public async Task RemoveVehicleAsync(Vehicle vehicle)
  {
    if (vehicle == null)
      throw new ArgumentNullException(nameof(vehicle));

    var existingVehicle = await _context.Vehicles.FindAsync(vehicle.Id);
    if (existingVehicle == null)
      throw new InvalidOperationException($"Veículo com ID {vehicle.Id} não encontrado no banco de dados.");

    _context.Vehicles.Remove(existingVehicle);
    await _context.SaveChangesAsync();
  }

  private void LogDatabaseConflict(EntityEntry entry, PropertyValues databaseValues)
  {
    // Registrar as diferenças entre os valores atuais e os valores no banco de dados
    foreach (var property in databaseValues.Properties)
    {
      var currentValue = entry.CurrentValues[property];
      var databaseValue = databaseValues[property];

      if (!Equals(currentValue, databaseValue))
      {
        Debug.WriteLine($"Conflito de concorrência: Propriedade {property.Name}: Valor atual = {currentValue}, Valor no banco = {databaseValue}");
      }
    }
  }
}