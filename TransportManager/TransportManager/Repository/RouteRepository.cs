namespace TransportManager.Repository;

public class RouteRepository : IRouteRepository
{
  private readonly TransportDbContext _context;

  public RouteRepository(TransportDbContext context)
  {
    _context = context ?? throw new ArgumentNullException(nameof(context));
  }

  public async Task<List<Route>> GetAllRoutesAsync()
  {
    return await _context.Routes
        .Include(r => r.Driver)
        .Include(r => r.Vehicle)
        .ToListAsync();
  }

  public async Task<Route?> GetRouteByIdAsync(string id)
  {
    return await _context.Routes
        .Include(r => r.Driver)
        .Include(r => r.Vehicle)
        .FirstOrDefaultAsync(r => r.Id == id);
  }

  public async Task AddRouteAsync(Route route)
  {
    if (route == null)
      throw new ArgumentNullException(nameof(route));

    await _context.Routes.AddAsync(route);
    await _context.SaveChangesAsync();
  }

  public async Task UpdateRouteAsync(Route route)
  {
    if (route == null)
      throw new ArgumentNullException(nameof(route));

    try
    {
      var existingRoute = await _context.Routes.FindAsync(route.Id);
      if (existingRoute == null)
        throw new InvalidOperationException($"Rota com ID {route.Id} não encontrada no banco de dados.");

      _context.Entry(existingRoute).CurrentValues.SetValues(route);

      // Atualizar relacionamentos
      if (route.Driver != null)
        existingRoute.Driver = route.Driver;

      if (route.Vehicle != null)
        existingRoute.Vehicle = route.Vehicle;

      await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException ex)
    {
      // Lidar com possíveis conflitos de concorrência
      var entry = ex.Entries.Single();
      var databaseValues = await entry.GetDatabaseValuesAsync();

      if (databaseValues == null)
        throw new InvalidOperationException("A rota foi removida por outro usuário.", ex);

      // Log das diferenças para diagnóstico
      LogDatabaseConflict(entry, databaseValues);

      throw; // Relançar a exceção para que a camada superior possa lidar com ela
    }
  }

  public async Task RemoveRouteAsync(Route route)
  {
    if (route == null)
      throw new ArgumentNullException(nameof(route));

    var existingRoute = await _context.Routes.FindAsync(route.Id);
    if (existingRoute == null)
      throw new InvalidOperationException($"Rota com ID {route.Id} não encontrada no banco de dados.");

    _context.Routes.Remove(existingRoute);
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