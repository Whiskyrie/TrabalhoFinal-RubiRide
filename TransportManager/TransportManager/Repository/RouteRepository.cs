namespace TransportManager.Repository;

public class RouteRepository {
  private readonly TransportDbContext _context;

  public RouteRepository(TransportDbContext context) { _context = context; }

  public async Task<List<Route>> GetAllRoutesAsync() {
    return await _context.Routes.Include(r => r.Driver)
        .Include(r => r.Vehicle)
        .ToListAsync();
  }

  public async Task AddRouteAsync(Route route) {
    await _context.Routes.AddAsync(route);
    await _context.SaveChangesAsync();
  }

  public async Task UpdateRouteAsync(Route route) {
    try {
      var existingRoute =
          await _context.Routes.FindAsync(route.Id) ?? throw new Exception(
              "A rota não foi encontrada no banco de dados.");
      _context.Entry(existingRoute).CurrentValues.SetValues(route);
      await _context.SaveChangesAsync();
    } catch (DbUpdateConcurrencyException ex) {
      var entry = ex.Entries.Single();
      var databaseValues =
          await entry.GetDatabaseValuesAsync()
          ?? throw new Exception("A rota foi removida por outro usuário.");
      var databaseRoute = (Route)databaseValues.ToObject();

      // Log das diferenças
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

  public async Task RemoveRouteAsync(Route route) {
    var existingRoute =
        await _context.Routes.FindAsync(route.Id)
        ?? throw new Exception("A rota não foi encontrada no banco de dados.");
    _context.Routes.Remove(existingRoute);
    await _context.SaveChangesAsync();
  }
}