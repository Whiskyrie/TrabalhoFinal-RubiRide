namespace TransportManager.Repository;

public class DriverRepository
(TransportDbContext context) {

  private readonly TransportDbContext _context = context;

  public async Task<List<Driver>> GetAllDriversAsync() {
    return await _context.Drivers.ToListAsync();
  }
  public async Task AddDriverAsync(Driver driver) {
    await _context.Drivers.AddAsync(driver);
    await _context.SaveChangesAsync();
  }

  public async Task UpdateDriverAsync(Driver driver) {
    try {
      var existingDriver =
          await _context.Drivers.FindAsync(driver.Id) ?? throw new Exception(
              "O motorista não foi encontrado no banco de dados.");
      _context.Entry(existingDriver).CurrentValues.SetValues(driver);
      await _context.SaveChangesAsync();
    } catch (DbUpdateConcurrencyException ex) {
      var entry = ex.Entries.Single();
      var databaseValues =
          await entry.GetDatabaseValuesAsync() ?? throw new Exception(
              "O motorista já foi removido por outro usuário.");
      var databaseDriver = (Driver)databaseValues.ToObject();

      // Log das Diferenças
      foreach (var property in databaseValues.Properties) {
        var currentValue = entry.CurrentValues[property];
        var databaseValue = databaseValues[property];
        if (!Equals(currentValue, databaseValue)) {
          System.Diagnostics.Debug.WriteLine(
              $"Propriedade {property.Name}: Valor atual = {currentValue}, Valor no banco = {databaseValue}");
        }
      }

      throw;
    }
  }
  public async Task RemoveDriverAsync(Driver driver) {
    var existingDriver =
        await _context.Drivers.FindAsync(driver.Id) ?? throw new Exception(
            "O motorista não foi encontrado no banco de dados.");
    _context.Drivers.Remove(existingDriver);
    await _context.SaveChangesAsync();
  }
}