using TransportManager.Services;

namespace TransportManager.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Registrar repositórios
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IDriverRepository, DriverRepository>();
        services.AddScoped<IRouteRepository, RouteRepository>();

        // Registrar serviços
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IDriverService, DriverService>();
        services.AddScoped<IRouteService, RouteService>();

        // Registrar serviços de cache
        services.AddSingleton<ICacheService, MemoryCacheService>();
        services.Decorate<IVehicleService, CachedVehicleService>();
        services.Decorate<IDriverService, CachedDriverService>();
        services.Decorate<IRouteService, CachedRouteService>();

        // REMOVIDO: Registrar validadores (não são mais necessários)

        // Registrar serviços adicionais
        services.AddTransient<Func<XamlRoot, IDialogService>>(
            provider => (xamlRoot) => new DialogService(xamlRoot));

        return services;
    }

    private static IServiceCollection Decorate<TService, TDecorator>(this IServiceCollection services)
        where TService : class
        where TDecorator : class, TService
    {
        // O mesmo código do método Decorate permanece...
        var serviceDescriptor = services.FirstOrDefault(sd => sd.ServiceType == typeof(TService));
        if (serviceDescriptor == null)
        {
            throw new InvalidOperationException($"Service of type {typeof(TService).Name} is not registered.");
        }

        services.Remove(serviceDescriptor);

        if (serviceDescriptor.ImplementationType != null)
        {
            services.Add(new ServiceDescriptor(
                serviceDescriptor.ImplementationType,
                serviceDescriptor.ImplementationType,
                serviceDescriptor.Lifetime));
        }

        services.Add(new ServiceDescriptor(
            typeof(TService),
            provider =>
            {
                var originalInstance = serviceDescriptor.ImplementationType != null
                    ? provider.GetRequiredService(serviceDescriptor.ImplementationType)
                    : serviceDescriptor.ImplementationFactory?.Invoke(provider);

                if (originalInstance == null)
                {
                    throw new InvalidOperationException($"Failed to resolve implementation for {typeof(TService).Name}.");
                }

                return ActivatorUtilities.CreateInstance(
                    provider,
                    typeof(TDecorator),
                    originalInstance);
            },
            serviceDescriptor.Lifetime));

        return services;
    }
}