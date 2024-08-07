namespace TransportManager.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TransportDbContext>
{
    public TransportDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TransportDbContext>();
        optionsBuilder.UseSqlite("Data Source=transport.db");

        return new TransportDbContext(optionsBuilder.Options);
    }
}
