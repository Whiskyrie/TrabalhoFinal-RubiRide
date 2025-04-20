namespace TransportManager.Data;

public static class DatabaseConfig
{

    public static string GetConnectionString(string environment)
    {
        return environment.ToLower() switch
        {
            "development" => "Data Source=transport.dev.db",
            "testing" => "Data Source=transport.test.db",
            _ => "Data Source=transport.db"
        };
    }

    public static IServiceCollection ConfigureDbContext(this IServiceCollection services, string environmentName)
    {
        var connectionString = GetConnectionString(environmentName);

        services.AddDbContext<TransportDbContext>(options =>
        {
            options.UseSqlite(connectionString);

            if (environmentName.Equals("development", StringComparison.OrdinalIgnoreCase))
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        return services;
    }
}