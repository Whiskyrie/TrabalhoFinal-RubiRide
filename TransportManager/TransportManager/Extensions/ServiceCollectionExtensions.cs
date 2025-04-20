using TransportManager.Services;
using TransportManager.Validation;

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

        // Registrar validadores
        services.AddTransient<IValidator<Vehicle>, VehicleValidator>();
        services.AddTransient<IValidator<Driver>, DriverValidator>();
        services.AddTransient<IValidator<Route>, RouteValidator>();

        // Registrar serviços adicionais
        services.AddTransient<Func<XamlRoot, IDialogService>>(
            provider => (xamlRoot) => new DialogService(xamlRoot));

        return services;
    }

    private static IServiceCollection Decorate<TService, TDecorator>(this IServiceCollection services)
        where TService : class
        where TDecorator : class, TService
    {
        // Obter o descritor do serviço original
        var serviceDescriptor = services.FirstOrDefault(sd => sd.ServiceType == typeof(TService));
        if (serviceDescriptor == null)
        {
            throw new InvalidOperationException($"Service of type {typeof(TService).Name} is not registered.");
        }

        // Remover o serviço original
        services.Remove(serviceDescriptor);

        // Registrar o serviço original com outro nome
        if (serviceDescriptor.ImplementationType != null)
        {
            services.Add(new ServiceDescriptor(
                serviceDescriptor.ImplementationType,
                serviceDescriptor.ImplementationType,
                serviceDescriptor.Lifetime));
        }

        // Registrar o decorador
        services.Add(new ServiceDescriptor(
            typeof(TService),
            provider =>
            {
                // Obter a instância do serviço original
                var originalInstance = serviceDescriptor.ImplementationType != null
                    ? provider.GetRequiredService(serviceDescriptor.ImplementationType)
                    : serviceDescriptor.ImplementationFactory?.Invoke(provider);

                if (originalInstance == null)
                {
                    throw new InvalidOperationException($"Failed to resolve implementation for {typeof(TService).Name}.");
                }

                // Criar o decorador
                return ActivatorUtilities.CreateInstance(
                    provider,
                    typeof(TDecorator),
                    originalInstance);
            },
            serviceDescriptor.Lifetime));

        return services;
    }
}