namespace TransportManager.Data;


public static class DatabaseInitializer
{

    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TransportDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TransportDbContext>>();

        try
        {
            logger.LogInformation("Verificando se o banco de dados precisa ser inicializado...");

            // Aplicar migrações pendentes
            await dbContext.Database.MigrateAsync();

            // Inicializar dados de exemplo
            await SeedVehiclesAsync(dbContext, logger);
            await SeedDriversAsync(dbContext, logger);
            await SeedRoutesAsync(dbContext, logger);

            logger.LogInformation("Inicialização do banco de dados concluída com sucesso.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ocorreu um erro durante a inicialização do banco de dados.");
            throw;
        }
    }

    private static async Task SeedVehiclesAsync(TransportDbContext dbContext, ILogger logger)
    {
        if (!await dbContext.Vehicles.AnyAsync())
        {
            logger.LogInformation("Adicionando veículos de exemplo...");

            var vehicles = new List<Vehicle>
            {
                new Vehicle
                {
                    Model = "Volkswagen Constellation 24.280",
                    Year = 2023,
                    LicensePlate = "ABC1234",
                    Capacity = 24.0,
                    Type = VehicleType.Truck,
                    Status = VehicleStatus.Available,
                    Name = "Truck 01"
                },
                new Vehicle
                {
                    Model = "Mercedes-Benz Sprinter",
                    Year = 2022,
                    LicensePlate = "DEF5678",
                    Capacity = 3.5,
                    Type = VehicleType.Van,
                    Status = VehicleStatus.Available,
                    Name = "Van 01"
                },
                new Vehicle
                {
                    Model = "Volvo FH 540",
                    Year = 2023,
                    LicensePlate = "GHI9012",
                    Capacity = 30.0,
                    Type = VehicleType.Truck,
                    Status = VehicleStatus.InUse,
                    Name = "Truck 02"
                }
            };

            await dbContext.Vehicles.AddRangeAsync(vehicles);
            await dbContext.SaveChangesAsync();
        }
    }

    private static async Task SeedDriversAsync(TransportDbContext dbContext, ILogger logger)
    {
        if (!await dbContext.Drivers.AnyAsync())
        {
            logger.LogInformation("Adicionando motoristas de exemplo...");

            var drivers = new List<Driver>
            {
                new Driver
                {
                    Name = "João Silva",
                    LicenseNumber = "12345678901234567890",
                    LicenseExpirationDate = DateTime.Now.AddYears(2),
                    Status = DriverStatus.Available
                },
                new Driver
                {
                    Name = "Maria Oliveira",
                    LicenseNumber = "09876543210987654321",
                    LicenseExpirationDate = DateTime.Now.AddYears(3),
                    Status = DriverStatus.Available
                },
                new Driver
                {
                    Name = "José Santos",
                    LicenseNumber = "15975346812975346812",
                    LicenseExpirationDate = DateTime.Now.AddYears(1),
                    Status = DriverStatus.OnDuty
                }
            };

            await dbContext.Drivers.AddRangeAsync(drivers);
            await dbContext.SaveChangesAsync();
        }
    }

    private static async Task SeedRoutesAsync(TransportDbContext dbContext, ILogger logger)
    {
        if (!await dbContext.Routes.AnyAsync())
        {
            // Verificar se existem motoristas e veículos para criar rotas
            var drivers = await dbContext.Drivers.ToListAsync();
            var vehicles = await dbContext.Vehicles.ToListAsync();

            if (drivers.Count > 0 && vehicles.Count > 0)
            {
                logger.LogInformation("Adicionando rotas de exemplo...");

                var cityDistances = new CityDistances();
                var routes = new List<Route>();

                // Criar algumas rotas de exemplo
                if (drivers.Count >= 1 && vehicles.Count >= 1)
                {
                    var route1 = new Route
                    {
                        StartLocation = "São Paulo",
                        EndLocation = "Rio de Janeiro",
                        Distance = cityDistances.GetDistance("São Paulo", "Rio de Janeiro"),
                        Driver = drivers[0],
                        Vehicle = vehicles[0],
                        Name = "Rota SP-RJ"
                    };
                    route1.CalculateEstimatedDuration();
                    routes.Add(route1);
                }

                if (drivers.Count >= 2 && vehicles.Count >= 2)
                {
                    var route2 = new Route
                    {
                        StartLocation = "São Paulo",
                        EndLocation = "Belo Horizonte",
                        Distance = cityDistances.GetDistance("São Paulo", "Belo Horizonte"),
                        Driver = drivers[1],
                        Vehicle = vehicles[1],
                        Name = "Rota SP-BH"
                    };
                    route2.CalculateEstimatedDuration();
                    routes.Add(route2);
                }

                if (routes.Count > 0)
                {
                    await dbContext.Routes.AddRangeAsync(routes);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}