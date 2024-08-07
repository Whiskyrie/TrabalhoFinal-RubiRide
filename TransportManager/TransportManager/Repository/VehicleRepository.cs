namespace TransportManager.Repository;

public class VehicleRepository
(TransportDbContext context) {
  private readonly TransportDbContext _context = context;

  public async Task<List<Vehicle>> GetAllVehiclesAsync() {
    return await _context.Vehicles.ToListAsync();
  }

  public async Task AddVehicleAsync(Vehicle vehicle) {
    await _context.Vehicles.AddAsync(vehicle);
    await _context.SaveChangesAsync();
  }

  public async Task UpdateVehicleAsync(Vehicle vehicle) {
    try {
      var existingVehicle = await _context.Vehicles.FindAsync(vehicle.Id);
      if (existingVehicle == null) {
        throw new Exception("O veículo não foi encontrado no banco de dados.");
      }

      _context.Entry(existingVehicle).CurrentValues.SetValues(vehicle);
      await _context.SaveChangesAsync();
    } catch (DbUpdateConcurrencyException ex) {
      var entry = ex.Entries.Single();
      var databaseValues =
          await entry.GetDatabaseValuesAsync()
          ?? throw new Exception("O veículo foi removido por outro usuário.");
      var databaseVehicle = (Vehicle)databaseValues.ToObject();
      // Log das diferenças
      foreach (var property in databaseValues.Properties) {
        var currentValue = entry.CurrentValues[property];
        var databaseValue = databaseValues[property];
        if (!Equals(currentValue, databaseValue)) {
          System.Diagnostics.Debug.WriteLine(
              $"Propriedade {property.Name}: Valor atual = {currentValue}, Valor no banco = {databaseValue}");
        }
      }

      throw; // Re-throw the exception for now
    }
  }
  public async Task RemoveVehicleAsync(Vehicle vehicle) {
    var existingVehicle =
        await _context.Vehicles.FindAsync(vehicle.Id) ?? throw new Exception(
            "O veículo não foi encontrado no banco de dados.");
    _context.Vehicles.Remove(existingVehicle);
    await _context.SaveChangesAsync();
  }
}