namespace TransportManager.Repository;

public class DriverRepository : IDriverRepository
{
  private readonly TransportDbContext _context;

  public DriverRepository(TransportDbContext context)
  {
    _context = context ?? throw new ArgumentNullException(nameof(context));
  }

  public async Task<List<Driver>> GetAllDriversAsync()
  {
    return await _context.Drivers.ToListAsync();
  }

  public async Task<Driver?> GetDriverByIdAsync(string id)
  {
    return await _context.Drivers.FindAsync(id);
  }

  public async Task AddDriverAsync(Driver driver)
  {
    if (driver == null)
      throw new ArgumentNullException(nameof(driver));

    await _context.Drivers.AddAsync(driver);
    await _context.SaveChangesAsync();
  }

  public async Task UpdateDriverAsync(Driver driver)
  {
    if (driver == null)
      throw new ArgumentNullException(nameof(driver));

    try
    {
      var existingDriver = await _context.Drivers.FindAsync(driver.Id);
      if (existingDriver == null)
        throw new InvalidOperationException($"Motorista com ID {driver.Id} não encontrado no banco de dados.");

      _context.Entry(existingDriver).CurrentValues.SetValues(driver);
      await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException ex)
    {
      // Lidar com possíveis conflitos de concorrência
      var entry = ex.Entries.Single();
      var databaseValues = await entry.GetDatabaseValuesAsync();

      if (databaseValues == null)
        throw new InvalidOperationException("O motorista foi removido por outro usuário.", ex);

      // Log das diferenças para diagnóstico
      LogDatabaseConflict(entry, databaseValues);

      throw; // Relançar a exceção para que a camada superior possa lidar com ela
    }
  }

  public async Task RemoveDriverAsync(Driver driver)
  {
    if (driver == null)
      throw new ArgumentNullException(nameof(driver));

    var existingDriver = await _context.Drivers.FindAsync(driver.Id);
    if (existingDriver == null)
      throw new InvalidOperationException($"Motorista com ID {driver.Id} não encontrado no banco de dados.");

    _context.Drivers.Remove(existingDriver);
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
